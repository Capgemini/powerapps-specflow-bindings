import { AuthenticatedRecordRepository, MetadataRepository, RecordRepository } from '../../src/repositories/index';

const fetchMock = require('fetch-mock/es5/client');

describe('CurrentUserRecordRepository', () => {
  const logicalName = 'account';

  let recordRepo: RecordRepository;
  let metadataRepo: jasmine.SpyObj<MetadataRepository>;
  beforeEach(() => {
    metadataRepo = jasmine.createSpyObj<MetadataRepository>('MetadataRepository',
      [
        'getEntitySetForEntity',
        'getEntityForLookupProperty',
        'getLookupPropertyForCollectionProperty',
        'getRelationshipMetadata',
      ]);
    metadataRepo.getEntitySetForEntity.and.returnValues(Promise.resolve('accounts'), Promise.resolve('contacts'));
    recordRepo = new AuthenticatedRecordRepository(metadataRepo, '', '');
  });

  afterEach(() => {
    fetchMock.restore();
  });

  describe('createRecord(entityLogicalName, record)', () => {
    it('returns a reference to the created record', async () => {
      const guid = 'bc148b3f-d7d7-4546-a3a8-d92ffd9a98c8';
      fetchMock.mock('*', { headers: { 'OData-EntityId': `api/data/v9.1/accounts(${guid})` } });

      const actualReference = await recordRepo.createRecord(logicalName, { });

      expect(actualReference).toEqual({ entityType: logicalName, id: guid });
    });
  });

  describe('retrieveRecord(entityLogicalName, id, query)', () => {
    it('returns the retrieved record', async () => {
      const expectedRecord = { accountid: 'account-id' };
      fetchMock.mock('*', expectedRecord);

      const actualRecord = await recordRepo.retrieveRecord('account', 'account-id');

      expect(actualRecord).toEqual(expectedRecord);
    });
  });

  describe('retrieveMultipleRecord(entityLogicalName, query)', () => {
    it('returns the retrieved records', async () => {
      const expectedResponse: Xrm.RetrieveMultipleResult = { entities: [], nextLink: '' };
      fetchMock.mock('*', expectedResponse);

      const actualResponse = await recordRepo.retrieveMultipleRecords('account', '?');

      expect(actualResponse).toEqual(expectedResponse);
    });
  });

  describe('upsertRecord(entityLogicalName, record)', () => {
    const record = { '@key': 'keyfield', keyfield: 'Test Key' };

    it('performs an update when a match is found on @key', async () => {
      const matchedRecordId = '<account-id>';
      const retrieveResult: Xrm.RetrieveMultipleResult = {
        entities: [{ accountid: matchedRecordId }],
        nextLink: '',
      };
      fetchMock.mock('*', retrieveResult);

      const result = await recordRepo.upsertRecord(logicalName, record);

      expect(result.id).toBe(matchedRecordId);
    });

    it('performs a create when no match is found on @key', async () => {
      const retrieveResult: Xrm.RetrieveMultipleResult = {
        entities: [],
        nextLink: '',
      };
      const id = 'account-id';
      fetchMock.get('*', retrieveResult);
      fetchMock.post('*', { headers: { 'OData-EntityId': `api/data/v9.1/accounts(${id})` } });

      const result = await recordRepo.upsertRecord(logicalName, {});

      expect(result.id).toBe(id);
    });
  });

  describe('deleteRecord(entityReference)', () => {
    it('returns a reference to the deleted record', async () => {
      const entityReference: Xrm.LookupValue = {
        entityType: 'account',
        id: '<account-id>',
      };
      fetchMock.mock('*', { status: 200 });

      const result = await recordRepo.deleteRecord(entityReference);

      expect(result).toEqual(entityReference);
    });
  });

  describe('associateManyToManyRecords(primaryRecord, relatedRecord, relationshipName)', () => {
    it('executes an associate request', async () => {
      const primary: Xrm.LookupValue = { id: '<primary-id>', entityType: 'primaryentity' };
      const related: Xrm.LookupValue[] = [{ id: '<related-id>', entityType: 'relatedentity' }];
      const relationship = 'primary_related';
      fetchMock.mock('*', { status: 200 });

      await recordRepo.associateManyToManyRecords(primary, related, relationship);
      const call = fetchMock.calls()[0];

      expect(call[0]).toBe('/api/data/v9.1/accounts(%3Cprimary-id%3E)/primary_related/$ref');
      expect(call[1].body).toBe('{"@odata.id":"contacts(<related-id>)"}');
    });
  });
});
