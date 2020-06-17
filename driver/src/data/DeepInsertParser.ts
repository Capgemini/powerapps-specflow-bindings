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
         * @param {IRecord} entity The deep insert object.
         * @returns {Promise<IDeepInsertResponse>} An async result containing references to all created records.
         * @memberof DeepInsertParser
         */
        public async deepInsert(logicalName: string, entity: IRecord): Promise<IDeepInsertResponse> {
            const associatedRecords: { alias?: string, reference: Xrm.LookupValue }[] = [];

            const lookupRecordsByNavProp = this.getManyToOneRecords(entity);
            const singleNavProps = Object.keys(lookupRecordsByNavProp);
            await Promise.all(singleNavProps.map(async (singleNavProp) => {
                const result = await this.createLookupRecord(logicalName, entity, lookupRecordsByNavProp, singleNavProp);
                associatedRecords.push(result.record, ...result.associatedRecords);
            }));

            const relatedRecordsByNavProp = this.getOneToManyRecords(entity);
            Object.keys(relatedRecordsByNavProp).forEach((collNavProp) => delete entity[collNavProp]);

            const rootRecord = await this.recordRepository.createRecord(logicalName, entity);

            await Promise.all(Object.keys(relatedRecordsByNavProp).map(async (collNavProp) => {
                const result = await this.createCollectionRecords(logicalName, rootRecord, relatedRecordsByNavProp, collNavProp);
                associatedRecords.push(...result);
            }));

            return {
                associatedRecords,
                record: {
                    alias: entity["@alias"] as string | undefined,
                    reference: rootRecord
                }
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

            const entityName = await this.metadataRepository.GetEntityForLookupProperty(logicalName, singleNavigationProperty);
            const deepInsertResponse = await this.deepInsert(entityName, navigationPropertyMap[singleNavigationProperty]);
            const entitySet = await this.metadataRepository.getEntitySetForEntity(entityName);

            entity[`${singleNavigationProperty}@odata.bind`] = `/${entitySet}(${deepInsertResponse.record.reference.id})`;

            return deepInsertResponse;
        }

        private async createCollectionRecords(
            logicalName: string,
            parentRecord: Xrm.LookupValue,
            navigationPropertyMap: { [navigationProperty: string]: IRecord[] },
            collectionNavigationProperty: string,
        ): Promise<{ alias: string, reference: Xrm.LookupValue }[]> {
            const entityName = await this.metadataRepository.GetEntityForCollectionProperty(logicalName, collectionNavigationProperty);
            const entitySet = await this.metadataRepository.getEntitySetForEntity(logicalName);
            const oppositeNavigationProperty = await this.metadataRepository.GetLookupPropertyForCollectionProperty(collectionNavigationProperty);

            const results = await Promise.all(navigationPropertyMap[collectionNavigationProperty].map(async (oneToManyRecord) => {
                oneToManyRecord[`${oppositeNavigationProperty}@odata.bind`] = `/${entitySet}(${parentRecord.id})`;
                const result = await this.deepInsert(entityName, oneToManyRecord);
                return [result.record, ...result.associatedRecords];
            }));

            return [].concat(...results as any);
        }
    }
}
