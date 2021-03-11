import { DeepInsertService, Record } from '../../src/data';
import { MetadataRepository, RecordRepository } from '../../src/repositories';

function mockRecord(
  metadataRepo: jasmine.SpyObj<MetadataRepository>,
  recordRepo: jasmine.SpyObj<RecordRepository>,
): Record {
  metadataRepo.getLookupPropertyForCollectionProperty.and.resolveTo('customerid_account');
  metadataRepo.getRelationshipMetadata.and.resolveTo({ RelationshipType: 'OneToManyRelationship' } as Xrm.Metadata.OneToNRelationshipMetadata);
  metadataRepo.getTargetsForLookupProperty.and.resolveTo(['contact']);
  recordRepo.upsertRecord
    .and.returnValues(
      Promise.resolve({ id: '<company-id>', entityType: 'account' }),
      Promise.resolve({ id: '<contact-id>', entityType: 'contact' }),
      Promise.resolve({ id: '<account-id>', entityType: 'account' }),
      Promise.resolve({ id: '<opportunity-id>', entityType: 'opportunity' }),
    );

  return {
    name: 'Sample Account',
    opportunity_customer_accounts: [
      {
        name: 'Test Opportunity associated to Sample Account',
      },
    ],
    primarycontactid: {
      firstname: 'John',
      lastname: 'Smith',
      masterid: {
        firstname: 'Jim',
        lastname: 'Smith',
      },
    },
  };
}

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
        'getTargetsForLookupProperty',
        'getLookupPropertyForCollectionProperty',
        'getRelationshipMetadata',
      ]);

    deepInsertService = new DeepInsertService(metadataRepo, recordRepo);
  });

  describe('.deepInsert(logicalName, record)', () => {
    it('creates each object in the object graph', async () => {
      const testRecord = mockRecord(metadataRepo, recordRepo);

      await deepInsertService.deepInsert('account', testRecord, {});

      expect(recordRepo.upsertRecord.calls.count()).toBe(4);
    });

    it('replaces nested objects with @odata.bind associations', async () => {
      const entitySetName = 'contacts';
      const contactId = '<contact-id>';
      metadataRepo.getTargetsForLookupProperty.and.resolveTo(['contact']);
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
      metadataRepo.getTargetsForLookupProperty.and.resolveTo(['contact']);
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
      metadataRepo.getTargetsForLookupProperty.and.resolveTo(['contact']);
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
      const testRecord = mockRecord(metadataRepo, newRecordRepo);

      await deepInsertService.deepInsert('account', testRecord, {}, newRecordRepo);

      expect(newRecordRepo.upsertRecord.calls.count()).toBe(4);
    });

    it('throws when the target entity can not be determined for a lookup record', async () => {
      metadataRepo.getTargetsForLookupProperty.and.resolveTo(['contact', 'account']);

      expectAsync(deepInsertService.deepInsert('account',
        {
          primarycontactid:
            {
              firstname: 'John',
              lastname: 'Smith',
            },
        },
        {})).toBeRejectedWithError('Unable to determine target entity for primarycontactid');
    });

    it('correctly determines the target entity for customer fields', async () => {
      metadataRepo.getTargetsForLookupProperty.withArgs('account', 'customer_contact').and.resolveTo(null);
      metadataRepo.getTargetsForLookupProperty.withArgs('account', 'customer').and.resolveTo(['contact', 'account']);
      recordRepo.upsertRecord
        .and.returnValues(
          Promise.resolve({ id: '<contact-id>', entityType: 'contact' }),
          Promise.resolve({ id: '<account-id>', entityType: 'account' }),
        );

      await deepInsertService.deepInsert('account',
        {
          customer_contact:
            {
              firstname: 'John',
              lastname: 'Smith',
            },
        },
        {});

      expect(recordRepo.upsertRecord.calls.first().args[0]).toBe('contact');
    });

    it('fallsback to @logicalName if targets are not found', async () => {
      metadataRepo.getTargetsForLookupProperty.and.resolveTo(['contact']);
      recordRepo.upsertRecord
        .and.returnValues(
          Promise.resolve({ id: '<contact-id>', entityType: 'contact' }),
          Promise.resolve({ id: '<account-id>', entityType: 'account' }),
        );

      await deepInsertService.deepInsert('account',
        {
          primarycontactid:
          {
            '@logicalName': 'contact',
            firstname: 'John',
            lastname: 'Smith',
          },
        },
        {});

      expect(recordRepo.upsertRecord.calls.count()).toBe(2);
    });
  });
});
