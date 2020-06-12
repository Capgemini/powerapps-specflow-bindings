/// <reference path="./data/DataManager.ts"/>
/// <reference path="./TestEvents.ts"/>

namespace Capgemini.Dynamics.Testing {
    /**
     * An instance of this class is injected by the automated UI testing framework to assist in test execution.
     *
     * @export
     * @class TestDriver
     */
    export class TestDriver {
        private readonly dataManager: Data.DataManager;
        private readonly testRecords: { [alias: string]: Xrm.LookupValue };

        /**
         * Creates an instance of TestDriver.
         * @param {TestDataManager} [dataManager] A test data manager.
         * @memberof TestDriver
         */
        constructor(dataManager?: Data.DataManager) {
            if (dataManager === undefined) {
                const recordRepository = new Data.RecordRepository(Xrm.WebApi.online);
                dataManager = new Data.DataManager(recordRepository, new Data.DeepInsertParser(
                    new Data.MetadataRepository(Xrm.WebApi.online),
                    recordRepository,
                ));
            }
            this.dataManager = dataManager;
            this.testRecords = {};

            this.registerListeners();
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
            const entityReference = await this.dataManager.createData(testRecord["@logicalName"], testRecord);

            this.testRecords[testRecord["@alias"]] = entityReference;

            return entityReference;
        }

        /**
         * Deletes all test data that has been created as a result of any requests to load data. @see loadJsonData
         *
         * @memberof TestDriver
         */
        public async deleteTestData(): Promise<void> {
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
            if (this.testRecords[alias] === undefined) {
                throw new Error(`Test record with alias '${alias}' does not exist`);
            }
            return Xrm.Navigation.openForm({
                entityId: this.testRecords[alias].id,
                entityName: this.testRecords[alias].entityType,
            });
        }

        private registerListeners(): void {
            window.top.addEventListener(TestEvents.LoadTestDataRequested, (e) => this.routeToListener(e, this.loadTestData.bind(this)));
            window.top.addEventListener(TestEvents.DeleteTestDataRequested, (e) => this.routeToListener(e, this.deleteTestData.bind(this)));
            window.top.addEventListener(TestEvents.OpenRecordRequested, (e) => this.routeToListener(e, this.openTestRecord.bind(this)));
        }

        private routeToListener(e: Event, listener: (...args: any[]) => void | PromiseLike<any>) {
            const detail = (e as CustomEvent).detail;

            const result = detail ? listener(detail.data) : listener();

            if (result && result.then !== undefined && detail && detail.callback !== undefined) {
                result.then(detail.callback);
            }
        }
    }
}
