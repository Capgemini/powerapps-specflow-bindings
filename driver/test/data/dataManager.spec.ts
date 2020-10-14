import { DataManager, DeepInsertService } from '../../src/data';
import { RecordRepository } from '../../src/repositories';

describe('TestDriver', () => {
  let recordRepository: jasmine.SpyObj<RecordRepository>;
  let deepInsertService: jasmine.SpyObj<DeepInsertService>;
  let dataManager: DataManager;
  beforeEach(() => {
    recordRepository = jasmine.createSpyObj<RecordRepository>(
      'RecordRepository', ['createRecord', 'deleteRecord'],
    );
    deepInsertService = jasmine.createSpyObj<DeepInsertService>('DeepInsertService', ['deepInsert']);
    dataManager = new DataManager(recordRepository, deepInsertService);
  });

  describe('.createData(record)', () => {
    it('tracks all created records', async () => {
      const assocRecs: { alias?: string, reference: Xrm.LookupValue }[] = [{
        reference: {
          entityType: 'opportunity',
          id: '<opportunity-id>',
        },
      }];
      const rec: { alias?: string, reference: Xrm.LookupValue } = {
        reference: {
          entityType: 'account',
          id: '<account-id>',
        },
      };
      deepInsertService.deepInsert.and.returnValue(Promise.resolve({
        associatedRecords: assocRecs,
        record: rec,
      }));

      await dataManager.createData('account',
        {
          name: 'Sample Account',
          primarycontactid:
          {
            firstname: 'John',
            lastname: 'Smith',
          },
        });

      expect(dataManager.refs).toEqual([rec.reference, ...assocRecs.map((r) => r.reference)]);
    });
  });

  describe('.cleanup()', () => {
    it('deletes the root record', async () => {
      deepInsertService.deepInsert.and.returnValue(Promise.resolve({
        associatedRecords: [],
        record: {
          reference: {
            entityType: 'account',
            id: '<account-id>',
          },
        },
      }));
      await dataManager.createData('account',
        {
          name: 'Sample Account',
        });

      await dataManager.cleanup();

      expect(recordRepository.deleteRecord.calls.count()).toBe(1);
    });

    it('deletes associated records', async () => {
      deepInsertService.deepInsert.and.returnValue(Promise.resolve({
        associatedRecords: [{
          reference: {
            entityType: 'opportunity',
            id: '<opportunity-id>',
          },
        }],
        record: {
          reference: {
            entityType: 'account',
            id: '<account-id>',
          },
        },
      }));
      await dataManager.createData('account',
        {
          name: 'Sample Account',
          primarycontactid:
          {
            firstname: 'John',
            lastname: 'Smith',
          },
        });

      await dataManager.cleanup();

      expect(recordRepository.deleteRecord.calls.count()).toBe(2);
    });

    it('does not attempt to delete previously deleted records', async () => {
      deepInsertService.deepInsert.and.returnValue(Promise.resolve({
        associatedRecords: [],
        record: {
          reference: {
            entityType: 'account',
            id: '<account-id>',
          },
        },
      }));
      await dataManager.createData('account',
        {
          name: 'Sample Account',
        });

      await dataManager.cleanup();

      expect(recordRepository.deleteRecord.calls.count()).toBe(1);
    });

    it('retries failed delete requests', async () => {
      deepInsertService.deepInsert.and.returnValue(Promise.resolve({
        associatedRecords: [],
        record: {
          reference: {
            entityType: 'account',
            id: '<account-id>',
          },
        },
      }));
      await dataManager.createData('account',
        {
          name: 'Sample Account',
        });
      recordRepository.deleteRecord.and.throwError('Failed to delete');

      dataManager.cleanup();

      expect(recordRepository.deleteRecord.calls.count()).toBe(3);
    });
  });
});
