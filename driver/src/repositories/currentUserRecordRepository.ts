import RecordRepository from './recordRepository';
import { Record } from '../data';
import { AssociateRequest } from '../requests';

/**
 * Repository to handle CRUD operations for entities.
 *
 * @export
 * @class RecordRepository
 * @extends {Repository}
 */
export default class CurrentUserRecordRepository implements RecordRepository {
  private readonly webApi: Xrm.WebApiOnline;

  /**
   * Creates an instance of CurrentUserRecordRepository.
   * @param webApi The web API instance.
   */
  constructor(webApi: Xrm.WebApiOnline) {
    this.webApi = webApi;
  }

  /**
   * Retrieves a record.
   * @param logicalName The logical name of the record to retrieve.
   * @param id The ID of the record to retrieve.
   * @param query The query string.
   */
  public async retrieveRecord(logicalName: string, id: string, query?: string): Promise<any> {
    return this.webApi.retrieveRecord(logicalName, id, query);
  }

  /**
   * Retrieves multiple records.
   * @param logicalName The logical name of the records to retrieve.
   * @param query The query string.
   */
  public async retrieveMultipleRecords(
    logicalName: string,
    query: string,
  ): Promise<Xrm.RetrieveMultipleResult> {
    return this.webApi.retrieveMultipleRecords(logicalName, query);
  }

  /**
     * Creates an entity record.
     *
     * @param {string} logicalName A logical name for the entity to create.
     * @param {Record} record A record to create.
     * @returns {Xrm.LookupValue} An entity reference to the created entity.
     * @memberof RecordRepository
     */
  public async createRecord(logicalName: string, record: Record): Promise<Xrm.LookupValue> {
    return this.webApi.createRecord(logicalName, record);
  }

  /**
     * Upserts an entity record.
     * @param {string} logicalName A logical name for the entity to upsert.
     * @param {Record} record A record to upsert.
     * @returns {Xrm.LookupValue} An entity reference to the upserted entity.
     * @memberof RecordRepository
     */
  public async upsertRecord(logicalName: string, record: Record): Promise<Xrm.LookupValue> {
    if (!record['@key']) {
      return this.webApi.createRecord(logicalName, record);
    }

    const retrieveResponse = await this.webApi.retrieveMultipleRecords(
      logicalName,
      `?$filter=${record['@key']} eq '${record[record['@key'] as string]}'&$select=${logicalName}id`,
    );

    if (retrieveResponse.entities.length > 0) {
      const id = retrieveResponse.entities[0][`${logicalName}id`];
      await this.webApi.updateRecord(logicalName, id, record);

      return { entityType: logicalName, id };
    }

    return this.webApi.createRecord(logicalName, record);
  }

  /**
     * Deletes an entity record.
     *
     * @param {Xrm.LookupValue} ref A reference to the entity to delete.
     * @returns {Xrm.LookupValue} A reference to the deleted entity.
     * @memberof RecordRepository
     */
  public async deleteRecord(ref: Xrm.LookupValue): Promise<Xrm.LookupValue> {
    return this.webApi.deleteRecord(ref.entityType, ref.id) as unknown as Xrm.LookupValue;
  }

  /**
     * Associates two records in a N:N Relationship.
     *
     * @param {Xrm.LookupValue} primaryRecord The Primary Record to associate.
     * @param {Xrm.LookupValue[]} relatedRecords The Related Records to associate.
     * @param {string} relationship The N:N Relationship Name.
     * @returns {Xrm.ExecuteResponse} The Response from the execute request.
     * @memberof RecordRepository
     */
  public async associateManyToManyRecords(
    primaryRecord: Xrm.LookupValue,
    relatedRecords: Xrm.LookupValue[],
    relationship: string,
  ): Promise<void> {
    this.webApi.execute(new AssociateRequest(primaryRecord, relatedRecords, relationship));
  }
}
