namespace Capgemini.Dynamics.Testing.Data {
    /**
     * Manages the creation and cleanup of data.
     *
     * @export
     * @class DataManager
     */
    export class DataManager {
        public readonly createdRecords: Xrm.LookupValue[];

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
            const deepInsertResponse = await this.deepInsertParser.deepInsert(logicalName, record);
            this.createdRecords.push(deepInsertResponse.record, ...deepInsertResponse.associatedRecords);

            return deepInsertResponse.record;
        }

        /**
         * Performs cleanup by deleting all records created via the TestDataManager.
         *
         * @returns {Promise<void>}
         * @memberof DataManager
         */
        public async cleanup(): Promise<void> {
            this.createdRecords.forEach(async (record) => {
                let retry = 0;
                while (retry < 3) {
                    try {
                        return await this.recordRepository.deleteRecord(record);
                    } catch (err) {
                        retry++;
                    }
                }
            });
            this.createdRecords.splice(0, this.createdRecords.length);
        }
    }
}
