import MetadataRepository from './metadataRepository';
import RecordRepository from './recordRepository';
import Record from '../data/record';

/**
 * Repository to handle CRUD operations for entities.
 *
 * @export
 * @class RecordRepository
 * @extends {Repository}
 */
export default class AuthenticatedRecordRepository implements RecordRepository {
  private readonly headers: { [header: string]: string };

  private readonly metadataRepo: MetadataRepository;

  /**
   * Creates an instance of AuthenticatedRecordRepository.
   * @param metadataRepo A metadata repository.
   * @param authToken The auth token for the impersonating user.
   * @param userToImpersonateId An optional ID for an impersonated user.
   */
  constructor(metadataRepo: MetadataRepository, authToken: string, userToImpersonateId?: string) {
    this.metadataRepo = metadataRepo;

    this.headers = {
      Authorization: `Bearer ${authToken}`,
      'Content-Type': 'application/json',
    };

    if (userToImpersonateId) {
      this.headers.CallerObjectId = userToImpersonateId;
    }
  }

  /**
   * Sets the user to impersonate.
   * @param userToImpersonateId The ID of the user to impersonate.
   */
  public setImpersonatedUserId(userToImpersonateId: string) {
    this.headers.CallerObjectId = userToImpersonateId;
  }

  /**
   * Retrieves a record.
   * @param logicalName The logical name of the record to retrieve.
   * @param id The ID of the record to retrieve.
   * @param query The query string.
   */
  public async retrieveRecord(logicalName: string, id: string, query?: string): Promise<any> {
    const entitySet = await this.metadataRepo.getEntitySetForEntity(logicalName);
    const res = await fetch(`api/data/v9.1/${entitySet}(${id})${query}`, { headers: this.headers });

    await AuthenticatedRecordRepository.checkResponseForError(res);

    return res.json();
  }

  /**
   * Retrieves multiple records.
   * @param logicalName The logical name of the records to retrieve.
   * @param query The query string.
   */
  public async retrieveMultipleRecords(logicalName: string, query?: string): Promise<any> {
    const entitySet = await this.metadataRepo.getEntitySetForEntity(logicalName);
    const res = await fetch(`api/data/v9.1/${entitySet}${query}`, { headers: this.headers });

    await AuthenticatedRecordRepository.checkResponseForError(res);

    return res.json();
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
    const entitySet = await this.metadataRepo.getEntitySetForEntity(logicalName);
    const res = await fetch(`api/data/v9.1/${entitySet}`, {
      headers: this.headers,
      body: JSON.stringify(record),
      method: 'POST',
    });

    await AuthenticatedRecordRepository.checkResponseForError(res);

    const id = res.headers.get('OData-EntityId')!.match(/\((.*)\)/)![1];
    return { entityType: logicalName, id };
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
      return this.createRecord(logicalName, record);
    }

    const retrieveResponse = await this.retrieveMultipleRecords(
      logicalName,
      `?$filter=${record['@key']} eq '${record[record['@key'] as string]}'&$select=${logicalName}id`,
    );

    if (retrieveResponse.entities.length > 0) {
      const id = retrieveResponse.entities[0][`${logicalName}id`];
      await this.updateRecord(logicalName, id, record);

      return { entityType: logicalName, id };
    }

    return this.createRecord(logicalName, record);
  }

  /**
     * Deletes an entity record.
     *
     * @param {Xrm.LookupValue} ref A reference to the entity to delete.
     * @returns {Xrm.LookupValue} A reference to the deleted entity.
     * @memberof RecordRepository
     */
  public async deleteRecord(ref: Xrm.LookupValue): Promise<Xrm.LookupValue> {
    const entitySet = await this.metadataRepo.getEntitySetForEntity(ref.entityType);
    const res = await fetch(`api/data/v9.1/${entitySet}(${ref.id})`,
      {
        headers: this.headers,
        method: 'DELETE',
      });

    await AuthenticatedRecordRepository.checkResponseForError(res);

    return ref;
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
    const entitySetA = await this.metadataRepo.getEntitySetForEntity(primaryRecord.entityType);
    const entitySetB = await this.metadataRepo.getEntitySetForEntity(relatedRecords[0].entityType);

    await Promise.all(relatedRecords.map(async (r) => {
      const res = await fetch(`api/data/v9.1/${entitySetA}(${primaryRecord.id})/${relationship}/$ref`,
        {
          headers: this.headers,
          body: JSON.stringify({ '@odata.id': `${entitySetB}(${r.id})` }),
          method: 'POST',
        });

      await AuthenticatedRecordRepository.checkResponseForError(res);
    }));
  }

  private async updateRecord(logicalName: string, id: string, record: any) {
    const entitySet = await this.metadataRepo.getEntitySetForEntity(logicalName);
    const res = await fetch(`api/data/v9.1/${entitySet}(${id})`,
      {
        headers: this.headers,
        body: JSON.stringify(record),
        method: 'PATCH',
      });

    await AuthenticatedRecordRepository.checkResponseForError(res);

    return res.json();
  }

  // eslint-disable-next-line no-undef
  private static async checkResponseForError(res: Response) {
    if (res.status >= 400) {
      const json = await res.json();
      throw new Error(`${json.error.code}: ${json.error.message}`);
    }
  }
}
