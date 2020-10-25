import { RecordRepository } from '../repositories';
import DeepInsertService from './deepInsertService';
import Preprocessor from './preprocesser';
import { Record } from './record';

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
     * @param {Record} record The record to deep insert.
     * @returns {Promise<Xrm.LookupValue>} An entity reference to the root record.
     * @memberof DataManager
     */
  public async createData(logicalName: string, record: Record): Promise<Xrm.LookupValue> {
    const res = await this.deepInsertSvc.deepInsert(
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

  /**
     * Performs cleanup by deleting all records created via the TestDataManager.
     *
     * @returns {Promise<void>}
     * @memberof DataManager
     */
  public async cleanup(): Promise<(Xrm.LookupValue | void)[]> {
    const deletePromises = this.refs.map(async (record) => {
      let reference;
      let retry = 0;
      while (retry < 3) {
        try {
          // eslint-disable-next-line no-await-in-loop
          reference = await this.recordRepo.deleteRecord(record);
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
