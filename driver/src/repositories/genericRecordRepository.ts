import RecordRepository from './recordRepository';
import Record from '../data/record';

/**
 * Generic repository to handle CRUD operations for entities.
 *
 * @export
 * @class GenericRecordRepository
 * @extends {RecordRepository}
 */
export default abstract class GenericRecordRepository implements RecordRepository {
  /**
   * Retrieves a record.
   * @param logicalName The logical name of the record to retrieve.
   * @param id The ID of the record to retrieve.
   * @param query The query string.
   */
  abstract retrieveRecord(logicalName: string, id: string, query?: string): Promise<any>;

  /**
  * Retrieves multiple records.
  * @param logicalName The logical name of the records to retrieve.
  * @param query The query string.
  */
  abstract retrieveMultipleRecords(logicalName: string, query?: string): Promise<Xrm.RetrieveMultipleResult>;

  /**
  * Creates an entity record.
  *
  * @param {string} logicalName A logical name for the entity to create.
  * @param {Record} record A record to create.
  * @returns {Xrm.LookupValue} An entity reference to the created entity.
  * @memberof RecordRepository
  */
  abstract createRecord(logicalName: string, record: Record): Promise<Xrm.LookupValue>;

  /**
   * Upserts an entity record.
   * @param {string} logicalName A logical name for the entity to upsert.
   * @param {Record} record A record to upsert.
   * @returns {Xrm.LookupValue} An entity reference to the upserted entity.
   * @memberof RecordRepository
   */
  abstract upsertRecord(logicalName: string, record: Record): Promise<Xrm.LookupValue>;

  /**
   * Deletes an entity record.
   *
   * @param {Xrm.LookupValue} ref A reference to the entity to delete.
   * @returns {Xrm.LookupValue} A reference to the deleted entity.
   * @memberof RecordRepository
   */
  abstract deleteRecord(ref: Xrm.LookupValue): Promise<Xrm.LookupValue>;

  /**
   * Associates two records in a N:N Relationship.
   *
   * @param {Xrm.LookupValue} primaryRecord The Primary Record to associate.
   * @param {Xrm.LookupValue[]} relatedRecords The Related Records to associate.
   * @param {string} relationship The N:N Relationship Name.
   * @returns {Xrm.ExecuteResponse} The Response from the execute request.
   * @memberof RecordRepository
   */
  abstract associateManyToManyRecords(
    primaryRecord: Xrm.LookupValue,
    relatedRecords: Xrm.LookupValue[],
    relationship: string
  ): Promise<void>;

  /**
   * Sanitises a record by removing any annotations unrecognised by the Web API.
   *
   * @param {Record} record The record to sanitise.
   * @returns {Record} The sanitised record.
   * @memberof RecordRepository
   */
  sanitiseRecord(record: Record): Record {
    return Object.fromEntries(
      Object.entries(record).filter(([key]) => !key.startsWith('@')),
    );
  }
}
