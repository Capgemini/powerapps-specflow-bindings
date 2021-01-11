import RecordRepository from '../repositories/recordRepository';
import AuthenticatedRecordRepository from '../repositories/authenticatedRecordRepository';
import { CreateOptions } from './createOptions';
import DeepInsertService from './deepInsertService';
import Preprocessor from './preprocessor';
import Record from './record';
import MetadataRepository from '../repositories/metadataRepository';

/**
 * Manages the creation and cleanup of data.
 *
 * @export
 * @class DataManager
 */
export default class DataManager {
  public readonly refs: Xrm.LookupValue[];

  public readonly refsByAlias: { [alias: string]: Xrm.LookupValue };

  private readonly recordRepo: RecordRepository;

  private readonly deepInsertSvc: DeepInsertService;

  private readonly preprocessors?: Preprocessor[];

  /**
     * Creates an instance of DataManager.
     * @param {RecordRepository} recordRepository A record repository.
     * @param {DeepInsertService} deepInsertService A deep insert parser.
     * @memberof DataManager
     */
  constructor(
    recordRepository: RecordRepository,
    deepInsertService: DeepInsertService,
    preprocessors?: Preprocessor[],
  ) {
    this.refs = [];
    this.refsByAlias = {};
    this.recordRepo = recordRepository;
    this.deepInsertSvc = deepInsertService;
    this.preprocessors = preprocessors;
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
    let svc = this.deepInsertSvc;

    if (opts?.authToken && opts?.userToImpersonate) {
      const azureAdObjectId = await this.getObjectIdForUser(opts.userToImpersonate);

      const metadataRepo = new MetadataRepository();
      svc = new DeepInsertService(
        metadataRepo,
        new AuthenticatedRecordRepository(
          metadataRepo,
          opts.authToken,
          azureAdObjectId,
        ),
      );
    }

    const res = await svc.deepInsert(
      logicalName,
      this.preprocess(record),
      this.refsByAlias,
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

  private async getObjectIdForUser(username: string): Promise<string> {
    const res = await this.recordRepo.retrieveMultipleRecords('systemuser', `?$filter=internalemailaddress eq '${username}'&$select=azureactivedirectoryobjectid`);

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
  public async cleanup(authToken?: string): Promise<(Xrm.LookupValue | void)[]> {
    const repo = authToken
      ? new AuthenticatedRecordRepository(new MetadataRepository(), authToken) : this.recordRepo;

    const deletePromises = this.refs.map(async (record) => {
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
