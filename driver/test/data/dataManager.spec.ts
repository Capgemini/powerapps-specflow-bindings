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
    const associatedRecord = { entityType: 'opportunity', id: '<opportunity-id>' };
    const associatedRecordAlias = 'an associated opportunity';
    const rootRecord = { entityType: 'account', id: '<account-id>' };
    const rootRecordAlias = 'an account';

    beforeEach(async () => {
      const mockDeepInsertResponse = Promise.resolve(
        {
          associatedRecords: [{ reference: associatedRecord, alias: associatedRecordAlias }],
          record: { reference: rootRecord, alias: rootRecordAlias },
        },
      );
      deepInsertService.deepInsert.and.returnValue(mockDeepInsertResponse);

      await dataManager.createData('', {});
    });

    it('deletes the root record', async () => {
      await dataManager.cleanup();

      expect(
        recordRepository.deleteRecord.calls.allArgs().find((args) => args[0].id === rootRecord.id),
      ).not.toBeNull();
    });

    it('deletes associated records', async () => {
      await dataManager.cleanup();

      expect(recordRepository.deleteRecord.calls.count()).toBe(2);
    });

    it('does not attempt to delete previously deleted records', async () => {
      await dataManager.cleanup();
      await dataManager.cleanup();

      expect(recordRepository.deleteRecord.calls.count()).toBe(2);
    });

    it('retries failed delete requests', async () => {
      recordRepository.deleteRecord.and.throwError('Failed to delete');

      dataManager.cleanup();

      expect(recordRepository.deleteRecord.calls.count()).toBe(6);
    });
  });
});
