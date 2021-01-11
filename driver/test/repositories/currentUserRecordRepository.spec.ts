import { CurrentUserRecordRepository, RecordRepository } from '../../src/repositories/index';
import { AssociateRequest } from '../../src/requests';

describe('CurrentUserRecordRepository', () => {
  let xrmWebApi: jasmine.SpyObj<Xrm.WebApiOnline>;
  let recordRepository: RecordRepository;
  beforeEach(() => {
    xrmWebApi = jasmine.createSpyObj<Xrm.WebApiOnline>('XrmWebApi', ['createRecord', 'deleteRecord', 'execute', 'retrieveMultipleRecords', 'updateRecord', 'retrieveRecord']);
    recordRepository = new CurrentUserRecordRepository(xrmWebApi);
  });

  describe('createRecord(entityLogicalName, record)', () => {
    it('returns a reference to the created record', async () => {
      const expectedReference: Xrm.CreateResponse = { entityType: 'account', id: '<account-id>' };
      xrmWebApi.createRecord.and.returnValue(Promise.resolve(expectedReference) as never);

      const actualReference = await recordRepository.createRecord('account', { name: 'Test Account' });

      expect(actualReference).toBe(expectedReference);
    });
  });

  describe('retrieveRecord(entityLogicalName, id, query)', () => {
    it('returns the retrieved record', async () => {
      const expectedRecord = {};
      xrmWebApi.retrieveRecord.and.returnValue(Promise.resolve(expectedRecord) as never);

      const actualRecord = await recordRepository.retrieveRecord('account', 'account-id');

      expect(actualRecord).toBe(expectedRecord);
    });

    it('passes the expected arguments to retrieveRecords', async () => {
      const logicalName = 'account';
      const id = 'id';
      const query = '?$select=accountid';

      await recordRepository.retrieveRecord(logicalName, id, query);

      expect(xrmWebApi.retrieveRecord).toHaveBeenCalledWith(logicalName, id, query);
    });
  });

  describe('retrieveMultipleRecord(entityLogicalName, query)', () => {
    it('returns the retrieved records', async () => {
      const expectedResponse: Xrm.RetrieveMultipleResult = { entities: [], nextLink: '' };
      xrmWebApi.retrieveMultipleRecords.and.returnValue(Promise.resolve(expectedResponse) as never);

      const actualResposne = await recordRepository.retrieveMultipleRecords('account', '?');

      expect(actualResposne).toBe(expectedResponse);
    });

    it('passes the expected arguments to retrieveMultipleRecords', async () => {
      const logicalName = 'account';
      const query = '?$select=accountid';

      await recordRepository.retrieveMultipleRecords(logicalName, query);

      expect(xrmWebApi.retrieveMultipleRecords).toHaveBeenCalledWith(logicalName, query);
    });
  });

  describe('upsertRecord(entityLogicalName, record)', () => {
    const record = { '@key': 'keyfield', keyfield: 'Test Key' };

    it('performs an update when a match is found on @key', async () => {
      const logicalName = 'account';
      const matchedRecordId = '<account-id>';
      const retrieveResult: Xrm.RetrieveMultipleResult = {
        entities: [{ accountid: matchedRecordId }],
        nextLink: '',
      };
      xrmWebApi.retrieveMultipleRecords.and.returnValue(Promise.resolve(retrieveResult) as never);

      await recordRepository.upsertRecord(logicalName, record);

      expect(xrmWebApi.updateRecord).toHaveBeenCalledWith(logicalName, matchedRecordId, record);
    });

    it('performs a create when no match is found on @key', async () => {
      const logicalName = 'account';
      xrmWebApi.retrieveMultipleRecords.and.returnValue(Promise.resolve({ entities: [], nextLink: '' }) as never);

      await recordRepository.upsertRecord(logicalName, record);

      expect(xrmWebApi.createRecord).toHaveBeenCalledWith(logicalName, record);
    });

    describe('deleteRecord(entityReference)', () => {
      it('returns a reference to the deleted record', async () => {
        const entityReference: Xrm.LookupValue = {
          entityType: 'account',
          id: '<account-id>',
        };
        xrmWebApi.deleteRecord.and.returnValue(Promise.resolve(entityReference) as never);

        const result = await recordRepository.deleteRecord(entityReference);

        expect(result).toBe(entityReference);
      });
    });

    describe('associateManyToManyRecords(primaryRecord, relatedRecord, relationshipName)', () => {
      it('executes an associate request', async () => {
        const primary: Xrm.LookupValue = { id: '<primary-id>', entityType: 'primaryentity' };
        const related: Xrm.LookupValue[] = [{ id: '<related-id>', entityType: 'relatedentity' }];
        const relationship = 'primary_related';

        await recordRepository.associateManyToManyRecords(primary, related, relationship);

        const actualRequest = xrmWebApi.execute.calls.first().args[0] as AssociateRequest;

        expect(actualRequest.target).toBe(primary);
        expect(actualRequest.relatedEntities).toBe(related);
        expect(actualRequest.relationship).toBe(relationship);
      });
    });
  });
});
