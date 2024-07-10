import { Record, RecordService } from '../../src/data';
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
    id: '123546879',
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

describe('RecordService', () => {
  let recordRepo: jasmine.SpyObj<RecordRepository>;
  let metadataRepo: jasmine.SpyObj<MetadataRepository>;
  let recordService: RecordService;

  beforeEach(() => {
    recordRepo = jasmine.createSpyObj<RecordRepository>('RecordRepository',
      [
        'deleteRecord',
        'upsertRecord',
        'associateManyToManyRecords',
        'retrieveRecord',
      ]);
    metadataRepo = jasmine.createSpyObj<MetadataRepository>('MetadataRepository',
      [
        'getEntitySetForEntity',
        'getTargetsForLookupProperty',
        'getLookupPropertyForCollectionProperty',
        'getRelationshipMetadata',
        'getPrimaryColumnName',
      ]);

    recordService = new RecordService(metadataRepo, recordRepo);
  });

  describe('.getExistingRecord(logicalName, id)', () => {
    it('get record', async () => {
      const testRecord = mockRecord(metadataRepo, recordRepo);

      const result = await recordService.getExistingRecord('account', testRecord?.id as string, metadataRepo, recordRepo);

      expect(result).toBeDefined();
    });
  });
});
