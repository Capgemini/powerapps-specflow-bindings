namespace Capgemini.Dynamics.Testing.Data {
    /**
     * Parses deep insert objects and returns references to all created records.
     *
     * @export
     * @class DeepInsertParser
     */
    export class DeepInsertParser {
        private readonly metadataRepository: MetadataRepository;
        private readonly recordRepository: RecordRepository;

        /**
         * Creates an instance of DeepInsertParser.
         * @param {MetadataRepository} metadataRepository A metadata repository.
         * @param {RecordRepository} recordRepository A record repostiroy.
         * @memberof DeepInsertParser
         */
        constructor(metadataRepository: MetadataRepository, recordRepository: RecordRepository) {
            this.metadataRepository = metadataRepository;
            this.recordRepository = recordRepository;
        }

        /**
         * A deep insert which returns a reference to all created records.
         *
         * @param {string} logicalName The entity logical name of the root record.
         * @param {(IRecord | IRecord)} entity The deep insert object.
         * @returns {Promise<IDeepInsertResponse>} An async result containing references to all created records.
         * @memberof DeepInsertParser
         */
        public async deepInsert(logicalName: string, entity: IRecord | IRecord): Promise<IDeepInsertResponse> {
            const associatedRecords = [] as Xrm.LookupValue[];

            const manyToOneRecordMap = this.getManyToOneRecords(entity);
            await Promise.all(Object.keys(manyToOneRecordMap).map(async (singleNavigationProperty) => {
                const result = await this.createLookupRecord(logicalName, entity, manyToOneRecordMap, singleNavigationProperty);
                associatedRecords.push(result.record, ...result.associatedRecords);
            }));

            const oneToManyRecordMap = this.getOneToManyRecords(entity);
            Object.keys(oneToManyRecordMap).forEach((collectionNavigationProperty) => delete entity[collectionNavigationProperty]);

            const record = await this.recordRepository.createRecord(logicalName, entity);

            await Promise.all(Object.keys(oneToManyRecordMap).map(async (collectionNavigationProperty) => {
                const result = await this.createCollectionRecords(logicalName, record, oneToManyRecordMap, collectionNavigationProperty);
                associatedRecords.push(...result);
            }));

            return {
                associatedRecords,
                record,
            };
        }

        private getOneToManyRecords(record: IRecord | IRecord): { [navigationProperty: string]: IRecord[] } {
            return Object.keys(record)
                .filter((key) => Array.isArray(record[key]))
                .reduce((prev, curr) => {
                    prev[curr] = record[curr] as IRecord[];
                    return prev;
                }, {} as { [navigationProperty: string]: IRecord[] });
        }

        private getManyToOneRecords(record: IRecord | IRecord): { [navigationProperty: string]: IRecord } {
            return Object.keys(record)
                .filter((key) => typeof record[key] === "object" && !Array.isArray(record[key]))
                .reduce((prev, curr) => {
                    prev[curr] = record[curr] as IRecord;
                    return prev;
                }, {} as { [navigationProperty: string]: IRecord });
        }

        private async createLookupRecord(
            logicalName: string,
            entity: IRecord,
            navigationPropertyMap: { [navigationProperty: string]: IRecord },
            singleNavigationProperty: string,
        ): Promise<IDeepInsertResponse> {
            delete entity[singleNavigationProperty];
            const entityLogicalName = await this.metadataRepository.GetEntityForLookupProperty(logicalName, singleNavigationProperty);
            const deepInsertResponse = await this.deepInsert(entityLogicalName, navigationPropertyMap[singleNavigationProperty]);
            const entitySet = await this.metadataRepository.getEntitySetForEntity(entityLogicalName);
            entity[`${singleNavigationProperty}@odata.bind`] = `/${entitySet}(${deepInsertResponse.record.id})`;

            return deepInsertResponse;
        }

        private async createCollectionRecords(
            logicalName: string,
            parentRecord: Xrm.LookupValue,
            navigationPropertyMap: { [navigationProperty: string]: IRecord[] },
            collectionNavigationProperty: string,
        ): Promise<Xrm.LookupValue[]> {
            const entityLogicalName = await this.metadataRepository.GetEntityForCollectionProperty(logicalName, collectionNavigationProperty);
            const entitySet = await this.metadataRepository.getEntitySetForEntity(logicalName);
            const oppositeNavigationProperty = await this.metadataRepository.GetLookupPropertyForCollectionProperty(collectionNavigationProperty);

            return Promise.all(navigationPropertyMap[collectionNavigationProperty].map((oneToManyRecord) => {
                oneToManyRecord[`${oppositeNavigationProperty}@odata.bind`] = `/${entitySet}(${parentRecord.id})`;
                return this.recordRepository.createRecord(entityLogicalName, oneToManyRecord);
            }));
        }
    }
}
