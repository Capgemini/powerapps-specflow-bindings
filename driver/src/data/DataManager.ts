namespace Capgemini.Dynamics.Testing.Data {
    /**
     * Manages the creation and cleanup of data.
     *
     * @export
     * @class DataManager
     */
    export class DataManager {
        public readonly createdRecords: Xrm.LookupValue[];
        public readonly createdRecordsByAlias: { [alias: string]: Xrm.LookupValue }
        private readonly recordRepository: RecordRepository;
        private readonly deepInsertParser: DeepInsertParser;

        /**
         * Creates an instance of DataManager.
         * @param {RecordRepository} recordRepository A record repository.
         * @param {DeepInsertParser} deepInsertParser A deep insert parser.
         * @memberof DataManager
         */
        constructor(recordRepository: RecordRepository, deepInsertParser: DeepInsertParser) {
            this.createdRecords = [];
            this.createdRecordsByAlias = {};
            this.recordRepository = recordRepository;
            this.deepInsertParser = deepInsertParser;
        }

        /**
         * Deep inserts a record for use in a test.
         *
         * @param {IRecord} record The record to deep insert.
         * @returns {Promise<Xrm.LookupValue>} An entity reference to the root record.
         * @memberof DataManager
         */
        public async createData(logicalName: string, record: IRecord): Promise<Xrm.LookupValue> {
            const deepInsertResponse = await this.deepInsertParser.deepInsert(logicalName, record, this.createdRecordsByAlias);
            const newRecords = [deepInsertResponse.record, ...deepInsertResponse.associatedRecords];

            this.createdRecords.push(...newRecords.map(r => r.reference));
            newRecords
                .filter(r => r.alias)
                .forEach(aliasedRecord => this.createdRecordsByAlias[aliasedRecord.alias!] = aliasedRecord.reference);

            return deepInsertResponse.record.reference;
        }

        /**
         * Performs cleanup by deleting all records created via the TestDataManager.
         *
         * @returns {Promise<void>}
         * @memberof DataManager
         */
        public async cleanup(): Promise<(Xrm.LookupValue | void)[]> {
            const deletePromises = this.createdRecords.map(async (record) => {
                let retry = 0;
                while (retry < 3) {
                    try {
                        const result = await this.recordRepository.deleteRecord(record);
                        return result;
                    } catch (err) {
                        retry++;
                    }
                }
            });
            this.createdRecords.splice(0, this.createdRecords.length);
            Object.keys(this.createdRecordsByAlias).forEach(alias => delete this.createdRecordsByAlias[alias]);
            return Promise.all(deletePromises);
        }
    }
}
