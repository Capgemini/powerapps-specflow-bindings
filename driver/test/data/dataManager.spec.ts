import { DataManager, DeepInsertService } from '../../src/data';
import { AuthenticatedRecordRepository, CurrentUserRecordRepository } from '../../src/repositories';

describe('TestDriver', () => {
  let currentUserRecordRepo: jasmine.SpyObj<CurrentUserRecordRepository>;
  let appUserRecordRepo: jasmine.SpyObj<AuthenticatedRecordRepository>;
  let deepInsertService: jasmine.SpyObj<DeepInsertService>;
  let dataManager: DataManager;

  beforeEach(() => {
    currentUserRecordRepo = jasmine.createSpyObj<CurrentUserRecordRepository>(
      'CurrentUserRecordRepository', ['createRecord', 'deleteRecord', 'retrieveMultipleRecords'],
    );
    appUserRecordRepo = jasmine.createSpyObj<AuthenticatedRecordRepository>(
      'AuthenticatedRecordRepository', ['createRecord', 'deleteRecord', 'setImpersonatedUserId'],
    );
    deepInsertService = jasmine.createSpyObj<DeepInsertService>('DeepInsertService', ['deepInsert']);
    dataManager = new DataManager(currentUserRecordRepo, deepInsertService, [], appUserRecordRepo);
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
      deepInsertService.deepInsert.and.resolveTo({
        associatedRecords: assocRecs,
        record: rec,
      });

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

    it('uses the application user repository if a user to impersonate is passed', async () => {
      const userToImpersonateId = 'user-id';
      currentUserRecordRepo.retrieveMultipleRecords.and.resolveTo(
        { entities: [{ azureactivedirectoryobjectid: 'user-id' }], nextLink: '' },
      );
      const records: { alias?: string, reference: Xrm.LookupValue } = {
        reference: { entityType: 'account', id: '<account-id>' },
      };
      deepInsertService.deepInsert.and.resolveTo({ record: records, associatedRecords: [] });

      await dataManager.createData('account', {}, { userToImpersonate: userToImpersonateId });

      expect(appUserRecordRepo.setImpersonatedUserId).toHaveBeenCalledWith(userToImpersonateId);
      expect(deepInsertService.deepInsert).toHaveBeenCalledWith(
        jasmine.anything(), jasmine.anything(), jasmine.anything(), appUserRecordRepo,
      );
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
        appUserRecordRepo.deleteRecord.calls.allArgs().find(
          (args) => args[0].id === rootRecord.id,
        ),
      ).not.toBeNull();
    });

    it('deletes associated records', async () => {
      await dataManager.cleanup();

      expect(appUserRecordRepo.deleteRecord.calls.count()).toBe(2);
    });

    it('does not attempt to delete previously deleted records', async () => {
      await dataManager.cleanup();
      await dataManager.cleanup();

      expect(appUserRecordRepo.deleteRecord.calls.count()).toBe(2);
    });

    it('retries failed delete requests', async () => {
      appUserRecordRepo.deleteRecord.and.throwError('Failed to delete');

      dataManager.cleanup();

      expect(appUserRecordRepo.deleteRecord.calls.count()).toBe(6);
    });

    it('uses the current user repository if no app user repository is set', async () => {
      const dm = new DataManager(currentUserRecordRepo, deepInsertService);
      const mockDeepInsertResponse = Promise.resolve(
        {
          associatedRecords: [{ reference: associatedRecord, alias: associatedRecordAlias }],
          record: { reference: rootRecord, alias: rootRecordAlias },
        },
      );
      deepInsertService.deepInsert.and.returnValue(mockDeepInsertResponse);
      await dm.createData('', {});

      await dm.cleanup();

      expect(currentUserRecordRepo.deleteRecord.calls.count()).toBe(2);
    });
  });
});
