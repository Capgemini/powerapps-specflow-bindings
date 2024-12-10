import Record from '../data/record';
import AssociateRequest from '../requests/associateRequest';
import GenericRecordRepository from './genericRecordRepository';

/**
 * Repository to handle CRUD operations for entities using the logged in user.
 *
 * @export
 * @class RecordRepository
 * @extends {Repository}
 */
export default class CurrentUserRecordRepository extends GenericRecordRepository {
  private readonly webApi: Xrm.WebApiOnline;

  /**
   * Creates an instance of CurrentUserRecordRepository.
   * @param webApi The web API instance.
   */
  constructor(webApi: Xrm.WebApiOnline) {
    super();

    this.webApi = webApi;
  }

  /** @inheritdoc */
  public async retrieveRecord(logicalName: string, id: string, query?: string): Promise<any> {
    return this.webApi.retrieveRecord(logicalName, id, query);
  }

  /** @inheritdoc */
  public async retrieveMultipleRecords(
    logicalName: string,
    query: string,
  ): Promise<Xrm.RetrieveMultipleResult> {
    return this.webApi.retrieveMultipleRecords(logicalName, query);
  }

  /** @inheritdoc */
  public async createRecord(logicalName: string, record: Record): Promise<Xrm.LookupValue> {
    return this.webApi.createRecord(logicalName, GenericRecordRepository.sanitiseRecord(record));
  }

  /** @inheritdoc */
  public async upsertRecord(logicalName: string, record: Record): Promise<Xrm.LookupValue> {
    if (!record['@key']) {
      return this.webApi.createRecord(
        logicalName, CurrentUserRecordRepository.sanitiseRecord(record),
      );
    }

    const retrieveResponse = await this.webApi.retrieveMultipleRecords(
      logicalName,
      `?$filter=${record['@key']} eq '${record[record['@key'] as string]}'&$select=${logicalName}id`,
    );

    if (retrieveResponse.entities.length > 0) {
      const id = retrieveResponse.entities[0][`${logicalName}id`];
      await this.webApi.updateRecord(
        logicalName, id, GenericRecordRepository.sanitiseRecord(record),
      );

      return { entityType: logicalName, id };
    }

    return this.webApi.createRecord(
      logicalName, GenericRecordRepository.sanitiseRecord(record),
    );
  }

  /** @inheritdoc */
  public async deleteRecord(ref: Xrm.LookupValue): Promise<Xrm.LookupValue> {
    return this.webApi.deleteRecord(ref.entityType, ref.id) as unknown as Xrm.LookupValue;
  }

  /** @inheritdoc */
  public async associateManyToManyRecords(
    primaryRecord: Xrm.LookupValue,
    relatedRecords: Xrm.LookupValue[],
    relationship: string,
  ): Promise<void> {
    this.webApi.execute(new AssociateRequest(primaryRecord, relatedRecords, relationship));
  }
}
