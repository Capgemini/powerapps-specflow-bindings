import AuthenticatedRecordRepository from '../repositories/authenticatedRecordRepository';
import { CreateOptions } from './createOptions';
import DeepInsertService from './deepInsertService';
import Preprocessor from './preprocessor';
import Record from './record';
import { CurrentUserRecordRepository } from '../repositories';
import RecordService from './recordService';

/**
 * Manages the creation and cleanup of data.
 *
 * @export
 * @class DataManager
 */
export default class DataManager {
  public readonly refs: Xrm.LookupValue[];

  public readonly preservedRefs: Xrm.LookupValue[];

  public readonly refsByAlias: { [alias: string]: Xrm.LookupValue };

  private readonly currentUserRecordRepo: CurrentUserRecordRepository;

  private readonly appUserRecordRepo?: AuthenticatedRecordRepository;

  private readonly deepInsertSvc: DeepInsertService;

  private readonly recordService: RecordService;

  private readonly preprocessors?: Preprocessor[];

  /**
       * Creates an instance of DataManager.
       * @param {RecordRepository} currentUserRecordRepo A record repository.
       * @param {DeepInsertService} deepInsertService A deep insert parser.
       * @param {Preprocessor} preprocessors Preprocessors that modify test data before creation.
       * @param {AuthenticatedRecordRepository} appUserRecordRepo An app user record repository
       * @memberof DataManager
       */
  constructor(
    currentUserRecordRepo: CurrentUserRecordRepository,
    deepInsertService: DeepInsertService,
    recordService: RecordService,
    preprocessors?: Preprocessor[],
    appUserRecordRepo?: AuthenticatedRecordRepository,
  ) {
    this.currentUserRecordRepo = currentUserRecordRepo;
    this.deepInsertSvc = deepInsertService;
    this.appUserRecordRepo = appUserRecordRepo;
    this.preprocessors = preprocessors;
    this.recordService = recordService;

    this.refs = [];
    this.preservedRefs = [];
    this.refsByAlias = {};
  }

  /**
       * Deep inserts a record for use in a test.
       *
       * @param {string} logicalName the logical name of the root entity.
       * @param {Record} record The record to deep insert.
       * @param {CreateOptions} opts options for creating the data.
       * @returns {Promise<Xrm.LookupValue>} An entity reference to the root record.
       * @memberof DataManager
       */
  public async createData(
    logicalName: string,
    record: Record,
    opts?: CreateOptions,
  ): Promise<Xrm.LookupValue> {
    if (opts?.userToImpersonate) {
      if (!this.appUserRecordRepo) {
        throw new Error('Unable to impersonate: an application user respository has not been configured.');
      }
      this.appUserRecordRepo.setImpersonatedUserId(
        await this.getObjectIdForUser(opts.userToImpersonate),
      );
    }
    const res = await this.deepInsertSvc.deepInsert(
      logicalName,
      this.preprocess(record),
      this.refsByAlias,
      opts?.userToImpersonate ? this.appUserRecordRepo : this.currentUserRecordRepo,
    );

    const newRecords = [res.record, ...res.associatedRecords];
    this.refs.push(...newRecords.map((r) => r.reference));
    newRecords
      .filter((r) => r.alias !== undefined)
      // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
      .forEach((aliasedRecord) => {
        this.refsByAlias[aliasedRecord.alias!] = aliasedRecord.reference;
      });

    return res.record.reference;
  }

  public async getData(
    logicalName: string,
    id: string,
    record: Record,
  ): Promise<Xrm.LookupValue> {
    const result = await this.recordService.getExistingRecord(logicalName, id);
    if (record?.['@alias']) {
      this.refsByAlias[record?.['@alias'] as string] = result;
    }

    this.refs.push(result);
    this.preservedRefs.push(result);
    return result;
  }

  private async getObjectIdForUser(username: string): Promise<string> {
    const res = await this.currentUserRecordRepo.retrieveMultipleRecords('systemuser', `?$filter=internalemailaddress eq '${username}'&$select=azureactivedirectoryobjectid`);

    if (res.entities.length === 0) {
      throw new Error(`Unable to impersonate ${username} as the user was not found.`);
    }

    return res.entities[0].azureactivedirectoryobjectid;
  }

  /**
       * Performs cleanup by deleting all records created via the TestDataManager.
       * @param authToken An optional auth token to use when deleting test data.
       * @returns {Promise<void>}
       * @memberof DataManager
       */
  public async cleanup(): Promise<(Xrm.LookupValue | void)[]> {
    const repo = this.appUserRecordRepo || this.currentUserRecordRepo;

    const cleanupRefs = this.refs.filter((x) => !this.preservedRefs.includes(x));
    const deletePromises = cleanupRefs.map(async (record) => {
      let reference;
      let retry = 0;
      while (retry < 3) {
        try {
          // eslint-disable-next-line no-await-in-loop
          reference = await repo.deleteRecord(record);
          break;
        } catch (err) {
          retry += 1;
        }
      }
      return reference;
    });

    this.refs.splice(0, this.refs.length);
    Object.keys(this.refsByAlias).forEach((alias) => delete this.refsByAlias[alias]);

    return Promise.all(deletePromises);
  }

  private preprocess(data: Record): Record {
    return this.preprocessors?.reduce<Record>(
      (record, preprocesser) => preprocesser.preprocess(record) ?? record,
      data,
    ) ?? data;
  }
}
