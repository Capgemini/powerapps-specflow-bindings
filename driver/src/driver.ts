import { DataManager } from './data';
import { TestRecord } from './data/testRecord';

/**
 * Interacts with the Web API and Client API to assist test setup and teardown.
 *
 * @export
 * @class Driver
 */
export default class Driver {
  private readonly dataManager: DataManager;

  /**
     * Creates an instance of Driver.
     * @param {TestDataManager} [dataManager] A test data manager.
     * @memberof Driver
     */
  constructor(dataManager: DataManager) {
    this.dataManager = dataManager;
  }

  /**
     * Loads test data into CDS from a JSON string. See Microsoft's docs on a 'Deep Insert'.
     * Should contain metadata to allow it to parse directly to an ITestRecord @see ITestRecord
     *
     * @param {string} json A JSON object.
     * @memberof Driver
     */
  public async loadTestData(json: string): Promise<Xrm.LookupValue> {
    const testRecord = JSON.parse(json) as TestRecord;
    const logicalName = testRecord['@logicalName'];

    return this.dataManager.createData(logicalName, testRecord);
  }

  /**
   *
   * @param json a JSON object.
   * @param userToImpersonate The username of the user to impersonate.
   */
  public async loadTestDataAsUser(
    json: string,
    userToImpersonate: string,
  ) {
    if (!userToImpersonate) {
      throw new Error('You have not provided the username of the user to impersonate.');
    }

    const testRecord = JSON.parse(json) as TestRecord;
    const logicalName = testRecord['@logicalName'];

    return this.dataManager.createData(logicalName, testRecord, { userToImpersonate });
  }

  /**
     * Deletes data that has been created as a result of any requests to load  @see loadJsonData
     * @memberof Driver
     */
  public deleteTestData(): Promise<(Xrm.LookupValue | void)[]> {
    return this.dataManager.cleanup();
  }

  /**
     * Opens a test record.
     *
     * @param {string} alias The alias of the test record.
     * @returns {Xrm.Async.PromiseLike<Xrm.Navigation.OpenFormResult} Open form result.
     * @memberof Driver
     */
  public openTestRecord(alias: string): Xrm.Async.PromiseLike<Xrm.Navigation.OpenFormResult> {
    if (this.dataManager.refsByAlias[alias] === undefined) {
      throw new Error(`Test record with alias '${alias}' does not exist`);
    }

    return Xrm.Navigation.openForm({
      entityId: this.dataManager.refsByAlias[alias].id,
      entityName: this.dataManager.refsByAlias[alias].entityType,
    });
  }

  // eslint-disable-next-line class-methods-use-this
  public async openForm(formName:string, nameOfEntity:string)
    :Promise<Xrm.Async.PromiseLike<Xrm.Navigation.OpenFormResult | void>> {
    const formInfo = await Xrm.WebApi.retrieveMultipleRecords('systemform', `?$select=formid&$filter=name eq '${formName}' and objecttypecode eq '${nameOfEntity}'`);
    if (formInfo.entities[0] === undefined) {
      throw new Error(`The specified form ('${formName}') for entity ('${nameOfEntity}') does not exist.`);
    }
    const formId = formInfo.entities[0].formid;
    const formType = formInfo.entities[0].type;
    const formPromise = Xrm.Navigation.openForm({
      entityName: nameOfEntity,
      formId,
      useQuickCreateForm: formType === 7,
    });

    if (formType === 7) {
      return Promise.resolve();
    }
    return formPromise;
  }

  /**
     * Gets a reference to a test record.
     * @param alias The alias of the test record.
     */
  public getRecordReference(alias: string): Xrm.LookupValue {
    return this.dataManager.refsByAlias[alias];
  }
}
