import Repository from './repository';
import { Record } from '../data/Record';
import { AssociateRequest } from '../requests/index';

/**
 * Repository to handle CRUD operations for entities.
 *
 * @export
 * @class RecordRepository
 * @extends {Repository}
 */
export default class RecordRepository extends Repository {
  /**
     * Creates an entity record.
     *
     * @param {string} entityLogicalName A logical name for the entity to create.
     * @param {Record} record A record to create.
     * @returns {Xrm.LookupValue} An entity reference to the created entity.
     * @memberof RecordRepository
     */
  public async createRecord(entityLogicalName: string, record: Record): Promise<Xrm.LookupValue> {
    if (record['@key']) {
      return this.upsertRecord(entityLogicalName, record);
    }

    return this.webApi.createRecord(entityLogicalName, record);
  }

  /**
     * Upserts an entity record.
     * @param {string} entityLogicalName A logical name for the entity to upsert.
     * @param {Record} record A record to upsert.
     * @returns {Xrm.LookupValue} An entity reference to the upserted entity.
     * @memberof RecordRepository
     */
  private async upsertRecord(entityLogicalName: string, record: Record): Promise<Xrm.LookupValue> {
    const retrieveResponse = await this.webApi.retrieveMultipleRecords(
      entityLogicalName,
      `?$filter=${record['@key']} eq '${record[record['@key'] as string]}'
                    &$select=${entityLogicalName}id`,
    );

    if (retrieveResponse.entities.length > 0) {
      const id = retrieveResponse.entities[0][`${entityLogicalName}id`];

      await this.webApi.updateRecord(entityLogicalName, id, record);

      return {
        entityType: entityLogicalName,
        id,
      };
    }

    return this.webApi.createRecord(entityLogicalName, record);
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
     * @param {string} relationshipName The N:N Relationship Name.
     * @returns {Xrm.ExecuteResponse} The Response from the execute request.
     * @memberof RecordRepository
     */
  public async associateManyToManyRecords(
    primaryRecord: Xrm.LookupValue,
    relatedRecords: Xrm.LookupValue[],
    relationshipName: string,
  ): Promise<Xrm.ExecuteResponse> {
    return this.webApi.execute(new AssociateRequest(
      primaryRecord,
      relatedRecords,
      relationshipName,
    ));
  }
}
