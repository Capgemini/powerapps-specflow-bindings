/// <reference path="./data/DataManager.ts"/>

namespace Capgemini.Dynamics.Testing {
    /**
     * An instance of this class is injected by the automated UI testing framework to assist in test execution.
     *
     * @export
     * @class TestDriver
     */
    export class TestDriver {
        private readonly dataManager: Data.DataManager;

        /**
         * Creates an instance of TestDriver.
         * @param {TestDataManager} [dataManager] A test data manager.
         * @memberof TestDriver
         */
        constructor(dataManager?: Data.DataManager) {
            if (dataManager === undefined) {
                const recordRepository = new Data.RecordRepository(Xrm.WebApi.online);
                dataManager = new Data.DataManager(
                    recordRepository,
                    new Data.DeepInsertParser(
                        new Data.MetadataRepository(Xrm.WebApi.online),
                        recordRepository,
                    )
                );
            }
            this.dataManager = dataManager;
        }

        /**
         * Loads test data into Dynamics 365 from a JSON string.
         * The JSON object can be one record or a graph of related records. See Microsoft's docs on a 'Deep Insert'.
         * It should contain additional metadata to allow it to parse directly to an ITestRecord @see ITestRecord
         *
         * @param {string} json A JSON object graph.
         * @memberof TestDriver
         */
        public async loadTestData(json: string): Promise<Xrm.LookupValue> {
            const testRecord = JSON.parse(json) as Data.ITestRecord;
            const logicalName = testRecord["@logicalName"];

            return this.dataManager.createData(logicalName, testRecord);
        }

        /**
         * Deletes all test data that has been created as a result of any requests to load data. @see loadJsonData
         *
         * @memberof TestDriver
         */
        public deleteTestData(): Promise<(Xrm.LookupValue | void)[]> {
            return this.dataManager.cleanup();
        }

        /**
         * Opens a test record.
         *
         * @param {string} alias The alias of the test record.
         * @returns {Xrm.Async.PromiseLike<Xrm.Navigation.OpenFormResult>} A promise containing the result.
         * @memberof TestDriver
         */
        public openTestRecord(alias: string): Xrm.Async.PromiseLike<Xrm.Navigation.OpenFormResult> {
            if (this.dataManager.createdRecordsByAlias[alias] === undefined) {
                throw new Error(`Test record with alias '${alias}' does not exist`);
            }

            return Xrm.Navigation.openForm({
                entityId: this.dataManager.createdRecordsByAlias[alias].id,
                entityName: this.dataManager.createdRecordsByAlias[alias].entityType,
            });
        }

        /**
         * Gets a reference to a test record.
         * @param alias The alias of the test record.
         */
        public getRecordReference(alias: string): Xrm.LookupValue {
            return this.dataManager.createdRecordsByAlias[alias];
        }
    }
}
