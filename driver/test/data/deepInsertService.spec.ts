import { DeepInsertService, Record } from '../../src/data';
import { MetadataRepository, RecordRepository } from '../../src/repositories';

describe('DeepInsertService', () => {
  let recordRepo: jasmine.SpyObj<RecordRepository>;
  let metadataRepo: jasmine.SpyObj<MetadataRepository>;
  let deepInsertService: DeepInsertService;

  beforeEach(() => {
    recordRepo = jasmine.createSpyObj<RecordRepository>('RecordRepository',
      [
        'createRecord',
        'deleteRecord',
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
      metadataRepo.getLookupPropertyForCollectionProperty.and.returnValue(Promise.resolve('customerid_account'));
      metadataRepo.getRelationshipMetadata.and.returnValue(Promise.resolve({ RelationshipType: 'OneToManyRelationship' } as Xrm.Metadata.OneToNRelationshipMetadata));
      recordRepo.createRecord
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

      expect(recordRepo.createRecord.calls.count()).toBe(4);
    });

    it('replaces nested objects with @odata.bind associations', async () => {
      const entitySetName = 'contacts';
      const contactId = '<contact-id>';
      metadataRepo.getEntitySetForEntity.and.returnValue(Promise.resolve(entitySetName));
      recordRepo.createRecord.and.returnValues(
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

      expect(recordRepo.createRecord.calls.mostRecent().args[1]['primarycontactid@odata.bind'])
        .toBe(`/${entitySetName}(${contactId})`);
    });

    it('replaces @alias.bind', async () => {
      const entitySetName = 'contacts';
      const contactId = '<contact-id>';
      const createdRecordsByAlias: { [alias: string]: Xrm.LookupValue } = { 'a contact': { id: contactId, entityType: 'contact' } };
      metadataRepo.getEntitySetForEntity.and.returnValue(Promise.resolve(entitySetName));

      await deepInsertService.deepInsert('account',
        {
          name: 'Sample Account',
          'primarycontactid@alias.bind': 'a contact',
        }, createdRecordsByAlias);

      expect(recordRepo.createRecord.calls.mostRecent().args[1]['primarycontactid@odata.bind'])
        .toBe(`/${entitySetName}(${contactId})`);
    });

    it('removes nested arrays from the record', async () => {
      metadataRepo.getEntitySetForEntity.and.returnValue(Promise.resolve('accounts'));
      metadataRepo.getRelationshipMetadata.and.returnValue(Promise.resolve({ RelationshipType: 'OneToManyRelationship' } as Xrm.Metadata.OneToNRelationshipMetadata));
      metadataRepo.getLookupPropertyForCollectionProperty.and.returnValue(Promise.resolve('customerid_account'));
      recordRepo.createRecord.and.returnValue(Promise.resolve({
        entityType: 'account',
        id: 'accountid',
      }));

      const testRecord: Record = {
        name: 'Sample Account',
        opportunity_customer_accounts: [
          {
            name: 'Test Opportunity associated to Sample Account',
          },
        ],
      };

      await deepInsertService.deepInsert('account', testRecord, {});

      expect(recordRepo.createRecord.calls.first().args[1].opportunity_customer_accounts)
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
        .returnValue(Promise.resolve(entitySet));
      metadataRepo.getRelationshipMetadata.and
        .returnValue(Promise.resolve({ RelationshipType: 'OneToManyRelationship' } as Xrm.Metadata.OneToNRelationshipMetadata));
      metadataRepo.getLookupPropertyForCollectionProperty.and
        .returnValue(Promise.resolve(navigationProperty));
      recordRepo.createRecord.and.returnValue(Promise.resolve(createdRecord));
      const testRecord: Record = {
        name: 'Sample Account',
        opportunity_customer_accounts: [
          {
            name: 'Test Opportunity associated to Sample Account',
          },
        ],
      };

      await deepInsertService.deepInsert('account', testRecord, {});

      expect(recordRepo.createRecord.calls.mostRecent().args[1][`${navigationProperty}@odata.bind`])
        .toBe(`/${entitySet}(${createdRecord.id})`);
    });

    it("queries for nested object's entity set name", async () => {
      metadataRepo.getEntitySetForEntity.and
        .returnValues(Promise.resolve('contacts'), Promise.resolve('accounts'));
      recordRepo.createRecord
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
      recordRepo.createRecord
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
  });
});
