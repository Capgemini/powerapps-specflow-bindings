import { RecordRepository } from '../../src/repositories/index';
import { AssociateRequest } from '../../src/requests';

describe('RecordRepository', () => {
  let xrmWebApi: jasmine.SpyObj<Xrm.WebApiOnline>;
  let recordRepository: RecordRepository;
  beforeEach(() => {
    xrmWebApi = jasmine.createSpyObj<Xrm.WebApiOnline>('XrmWebApi', ['createRecord', 'deleteRecord', 'execute', 'retrieveMultipleRecords', 'updateRecord']);
    recordRepository = new RecordRepository(xrmWebApi);
  });

  describe('createRecord(entityLogicalName, record)', () => {
    it('returns a reference to the created record', async () => {
      const expectedReference: Xrm.CreateResponse = { entityType: 'account', id: '<account-id>' };
      xrmWebApi.createRecord.and.returnValue(Promise.resolve(expectedReference) as never);

      const actualReference = await recordRepository.createRecord('account', { name: 'Test Account' });

      expect(actualReference).toBe(expectedReference);
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
