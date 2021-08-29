import { CreateOptions } from '../src/data/createOptions';
import DataManager from '../src/data/dataManager';
import Driver from '../src/driver';

describe('Driver', () => {
  let dataManager: jasmine.SpyObj<DataManager>;
  let driver: Driver;
  beforeAll(() => {
    dataManager = jasmine.createSpyObj<DataManager>('DataManager', ['createData', 'cleanup', 'refs', 'refsByAlias']);
    driver = new Driver(dataManager);
  });

  describe('.loadTestDataAsUser(json, username)', () => {
    it('throws if a username isn\'t provided', () => expectAsync(driver.loadTestDataAsUser('{}', '')).toBeRejectedWithError(/.*username.*/));

    it('passes CreateOptions to dataManager', async () => {
      const userToImpersonate = 'user@contoso.com';
      const expectedCreateOptions: CreateOptions = { userToImpersonate };

      await driver.loadTestDataAsUser('{ "@logicalName": "contact" }', userToImpersonate);

      expect(dataManager.createData).toHaveBeenCalledWith(
        jasmine.anything(),
        jasmine.anything(),
        jasmine.objectContaining(expectedCreateOptions),
      );
    });
  });

  describe('.loadJsonData(json)', () => {
    it('uses the TestDataManager to create the test data', () => {
      const logicalName = 'account';
      const testData = {
        '@logicalName': logicalName,
        name: 'Sample Account',
        primarycontactid:
        {
          firstname: 'John',
          lastname: 'Smith',
        },
      };
      const createDataCalls = dataManager.createData.calls.count();

      driver.loadTestData(JSON.stringify(testData));

      expect(dataManager.createData).toHaveBeenCalledTimes(createDataCalls + 1);
    });
  });

  describe('.deleteTestData()', () => {
    it('uses the TestDataManager to delete the test data', () => {
      dataManager.cleanup.calls.reset();
      driver.deleteTestData();

      expect(dataManager.cleanup.calls.count()).toBe(1);
    });
  });

  describe('.openTestRecord(alias)', () => {
    it('throws an error the record hasn\'t been created', () => {
      expect(() => driver.openTestRecord('this is not a valid alias')).toThrowError();
    });

    it('navigates to a record', async () => {
      const openForm = jasmine.createSpy('openForm');
      window.Xrm = {
        Navigation: {
          openForm,
        },
      } as unknown as Xrm.XrmStatic;
      dataManager.refsByAlias.someAlias = { id: '<account-id>', entityType: 'account' };

      driver.openTestRecord('someAlias');

      expect(openForm.calls.first().args[0].entityId).toBe('<account-id>');
    });
  });

  describe('.openForm(formName, nameOfEntity)', () => {
    it('throws an error if the form or entity do not exist', () => {
      const entities = [undefined];
      const returnObj = { entities };
      const retrieveMultipleRecords = jasmine.createSpy('retrieveMultipleRecords').and.returnValue(returnObj);
      const openForm = jasmine.createSpy('openForm');
      window.Xrm = {
        WebApi: {
          retrieveMultipleRecords,
        },
        Navigation: {
          openForm,
        },
      } as unknown as Xrm.XrmStatic;
      driver.openForm('Account Form', 'account');

      expectAsync(driver.openForm('Account Form', 'account')).toBeRejectedWithError('The specified form (\'Account Form\') for entity (\'account\') does not exist.');
    });

    it('opens a form for a given entity', () => {
      const formid = 'fakeid';
      const type = 2;
      const entity = { formid, type };
      const entities = [entity];
      const returnObj = { entities };
      const retrieveMultipleRecords = jasmine.createSpy('retrieveMultipleRecords').and.returnValue(returnObj);
      const openForm = jasmine.createSpy('openForm');
      window.Xrm = {
        WebApi: {
          retrieveMultipleRecords,
        },
        Navigation: {
          openForm,
        },
      } as unknown as Xrm.XrmStatic;

      expectAsync(driver.openForm('Account Form', 'account')).toBeResolved();
    });

    it('opens a quick create form for a given entity', () => {
      const formid = 'fakeid';
      const type = 7;
      const entity = { formid, type };
      const entities = [entity];
      const returnObj = { entities };
      const retrieveMultipleRecords = jasmine.createSpy('retrieveMultipleRecords').and.returnValue(returnObj);
      const openForm = jasmine.createSpy('openForm');
      window.Xrm = {
        WebApi: {
          retrieveMultipleRecords,
        },
        Navigation: {
          openForm,
        },
      } as unknown as Xrm.XrmStatic;

      expectAsync(driver.openForm('Account Form', 'account')).toBeResolved();
    });
  });

  describe('.getRecordReference(alias)', () => {
    it('returns a reference to a record matching the provided alias', () => {
      const alias = 'some alias';
      const reference: Xrm.LookupValue = { id: 'id', entityType: 'entitytype' };
      dataManager.refsByAlias[alias] = reference;

      expect(driver.getRecordReference(alias)).toBe(reference);
    });
  });
});
