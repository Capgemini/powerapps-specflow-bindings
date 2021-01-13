import { DeepInsertService, Record } from '../../src/data';
import { MetadataRepository, RecordRepository } from '../../src/repositories';

describe('DeepInsertService', () => {
  let recordRepo: jasmine.SpyObj<RecordRepository>;
  let metadataRepo: jasmine.SpyObj<MetadataRepository>;
  let deepInsertService: DeepInsertService;

  beforeEach(() => {
    recordRepo = jasmine.createSpyObj<RecordRepository>('RecordRepository',
      [
        'deleteRecord',
        'upsertRecord',
        'associateManyToManyRecords',
      ]);
    metadataRepo = jasmine.createSpyObj<MetadataRepository>('MetadataRepository',
      [
        'getEntitySetForEntity',
        'getEntityForLookupProperty',
        'getLookupPropertyForCollectionProperty',
        'getRelationshipMetadata',
      ]);

    deepInsertService = new DeepInsertService(metadataRepo, recordRepo);
  });

  describe('.deepInsert(logicalName, record)', () => {
    it('creates each object in the object graph', async () => {
      metadataRepo.getLookupPropertyForCollectionProperty.and.resolveTo('customerid_account');
      metadataRepo.getRelationshipMetadata.and.resolveTo({ RelationshipType: 'OneToManyRelationship' } as Xrm.Metadata.OneToNRelationshipMetadata);
      recordRepo.upsertRecord
        .and.returnValues(
          Promise.resolve({ id: '<company-id>', entityType: 'account' }),
          Promise.resolve({ id: '<contact-id>', entityType: 'contact' }),
          Promise.resolve({ id: '<account-id>', entityType: 'account' }),
          Promise.resolve({ id: '<opportunity-id>', entityType: 'opportunity' }),
        );

      const testRecord: Record = {
        name: 'Sample Account',
        opportunity_customer_accounts: [
          {
            name: 'Test Opportunity associated to Sample Account',
          },
        ],
        primarycontactid:
        {
          firstname: 'John',
          lastname: 'Smith',
          masterid: {
            firstname: 'Jim',
            lastname: 'Smith',
          },
        },
      };

      await deepInsertService.deepInsert('account', testRecord, {});

      expect(recordRepo.upsertRecord.calls.count()).toBe(4);
    });

    it('replaces nested objects with @odata.bind associations', async () => {
      const entitySetName = 'contacts';
      const contactId = '<contact-id>';
      metadataRepo.getEntitySetForEntity.and.resolveTo(entitySetName);
      recordRepo.upsertRecord.and.returnValues(
        Promise.resolve({ id: contactId, entityType: 'contact' }),
        Promise.resolve({ id: '<account-id>', entityType: 'account' }),
      );

      await deepInsertService.deepInsert('account',
        {
          name: 'Sample Account',
          primarycontactid:
          {
            firstname: 'John',
            lastname: 'Smith',
          },
        },
        {});

      expect(recordRepo.upsertRecord.calls.mostRecent().args[1]['primarycontactid@odata.bind'])
        .toBe(`/${entitySetName}(${contactId})`);
    });

    it('replaces @alias.bind', async () => {
      const entitySetName = 'contacts';
      const contactId = '<contact-id>';
      const createdRecordsByAlias: { [alias: string]: Xrm.LookupValue } = { 'a contact': { id: contactId, entityType: 'contact' } };
      metadataRepo.getEntitySetForEntity.and.resolveTo(entitySetName);

      await deepInsertService.deepInsert('account',
        {
          name: 'Sample Account',
          'primarycontactid@alias.bind': 'a contact',
        }, createdRecordsByAlias);

      expect(recordRepo.upsertRecord.calls.mostRecent().args[1]['primarycontactid@odata.bind'])
        .toBe(`/${entitySetName}(${contactId})`);
    });

    it('throws an error when @alias.bind alias is not found', async () => {
      const deepInsert = {
        name: 'Sample Account',
        'primarycontactid@alias.bind': 'a contact',
      };

      const deepInsertPromise = deepInsertService.deepInsert('account', deepInsert, {});

      expectAsync(deepInsertPromise).toBeRejectedWithError(Error, /.* record with the alias 'a contact' has not been created/);
    });

    it('removes nested arrays from the record', async () => {
      metadataRepo.getEntitySetForEntity.and.resolveTo('accounts');
      metadataRepo.getRelationshipMetadata.and.resolveTo({ RelationshipType: 'OneToManyRelationship' } as Xrm.Metadata.OneToNRelationshipMetadata);
      metadataRepo.getLookupPropertyForCollectionProperty.and.resolveTo('customerid_account');
      recordRepo.upsertRecord.and.resolveTo({
        entityType: 'account',
        id: 'accountid',
      });

      const testRecord: Record = {
        name: 'Sample Account',
        opportunity_customer_accounts: [
          {
            name: 'Test Opportunity associated to Sample Account',
          },
        ],
      };

      await deepInsertService.deepInsert('account', testRecord, {});

      expect(recordRepo.upsertRecord.calls.first().args[1].opportunity_customer_accounts)
        .toBeUndefined();
    });

    it('creates records in nested arrays with @odata.bind associations back to the parent object', async () => {
      const navigationProperty = 'customerid_account';
      const entitySet = 'accounts';
      const createdRecord: Xrm.LookupValue = {
        entityType: 'account',
        id: 'accountid',
      };
      metadataRepo.getEntitySetForEntity.and
        .resolveTo(entitySet);
      metadataRepo.getRelationshipMetadata.and
        .resolveTo({ RelationshipType: 'OneToManyRelationship' } as Xrm.Metadata.OneToNRelationshipMetadata);
      metadataRepo.getLookupPropertyForCollectionProperty.and
        .resolveTo(navigationProperty);
      recordRepo.upsertRecord.and.resolveTo(createdRecord);
      const testRecord: Record = {
        name: 'Sample Account',
        opportunity_customer_accounts: [
          {
            name: 'Test Opportunity associated to Sample Account',
          },
        ],
      };

      await deepInsertService.deepInsert('account', testRecord, {});

      expect(recordRepo.upsertRecord.calls.mostRecent().args[1][`${navigationProperty}@odata.bind`])
        .toBe(`/${entitySet}(${createdRecord.id})`);
    });

    it('creates and associates many-to-many records', async () => {
      const manyToManyNavigationProp = 'many_to_many';
      const relatedRecords = [{ name: 'Many to many record' }];
      const rootEntitySet = 'accounts';
      const rootReference: Xrm.LookupValue = { entityType: 'account', id: 'accountid' };
      const relatedReference: Xrm.LookupValue = { entityType: 'contact', id: 'contactid' };
      metadataRepo.getEntitySetForEntity.and.resolveTo(rootEntitySet);
      metadataRepo.getRelationshipMetadata.and.resolveTo({ RelationshipType: 'ManyToManyRelationship' } as Xrm.Metadata.NToNRelationshipMetadata);
      metadataRepo.getLookupPropertyForCollectionProperty.and.resolveTo(manyToManyNavigationProp);
      recordRepo.upsertRecord.and.returnValues(
        Promise.resolve(rootReference),
        Promise.resolve(relatedReference),
      );
      const testRecord: Record = { name: 'Account', [manyToManyNavigationProp]: relatedRecords };

      await deepInsertService.deepInsert('account', testRecord, {});

      expect(recordRepo.associateManyToManyRecords)
        .toHaveBeenCalledWith(rootReference, [relatedReference], manyToManyNavigationProp);
    });

    it("queries for nested object's entity set name", async () => {
      metadataRepo.getEntitySetForEntity.and
        .returnValues(Promise.resolve('contacts'), Promise.resolve('accounts'));
      recordRepo.upsertRecord
        .and.returnValues(
          Promise.resolve({ id: '<contact-id>', entityType: 'contact' }),
          Promise.resolve({ id: '<account-id>', entityType: 'account' }),
        );

      await deepInsertService.deepInsert('account',
        {
          name: 'Sample Account',
          primarycontactid:
          {
            firstname: 'John',
            lastname: 'Smith',
          },
        },
        {});

      expect(metadataRepo.getEntitySetForEntity).toHaveBeenCalledTimes(1);
    });

    it('returns an entity reference for each created record', async () => {
      const expectedEntityReference = { id: '<contact-id>', entityType: 'contact' };
      const createResponses = [
        Promise.resolve({ id: '<account-id>', entityType: 'account' }),
        Promise.resolve(expectedEntityReference),
      ];
      recordRepo.upsertRecord
        .and.returnValues(...createResponses);

      const entityReference = await deepInsertService.deepInsert('account',
        {
          name: 'Sample Account',
          primarycontactid:
          {
            firstname: 'John',
            lastname: 'Smith',
          },
        },
        {});

      expect(entityReference.record.reference).toBe(expectedEntityReference);
    });

    it('overrides the default repository with the one passed as an argument (if provided)', async () => {
      const newRecordRepo = jasmine.createSpyObj<RecordRepository>('RecordRepository', ['upsertRecord']);

      await deepInsertService.deepInsert('account', { name: 'Sample Account' }, {}, newRecordRepo);

      expect(newRecordRepo.upsertRecord).toHaveBeenCalledTimes(1);
    });
  });
});
