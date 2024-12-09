import MetadataRepository from './metadataRepository';
import Record from '../data/record';
import GenericRecordRepository from './genericRecordRepository';

/**
 * Repository to handle CRUD operations for entities with authentication and impersonation.
 *
 * @export
 * @class RecordRepository
 * @extends {Repository}
 */
export default class AuthenticatedRecordRepository extends GenericRecordRepository {
  private readonly headers: { [header: string]: string };

  private readonly metadataRepo: MetadataRepository;

  /**
   * Creates an instance of AuthenticatedRecordRepository.
   * @param metadataRepo A metadata repository.
   * @param authToken The auth token for the impersonating user.
   * @param userToImpersonateId An optional ID for an impersonated user.
   */
  constructor(metadataRepo: MetadataRepository, authToken: string, userToImpersonateId?: string) {
    super();

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

  /** @inheritdoc */
  public async retrieveRecord(logicalName: string, id: string, query?: string): Promise<any> {
    const entitySet = await this.metadataRepo.getEntitySetForEntity(logicalName);
    const res = await fetch(`api/data/v9.1/${entitySet}(${id})${query}`, { headers: this.headers });

    await AuthenticatedRecordRepository.checkResponseForError(res);

    return res.json();
  }

  /** @inheritdoc */
  public async retrieveMultipleRecords(logicalName: string, query?: string): Promise<any> {
    const entitySet = await this.metadataRepo.getEntitySetForEntity(logicalName);
    const res = await fetch(`api/data/v9.1/${entitySet}${query}`, { headers: this.headers });

    await AuthenticatedRecordRepository.checkResponseForError(res);

    return res.json();
  }

  /** @inheritdoc */
  public async createRecord(logicalName: string, record: Record): Promise<Xrm.LookupValue> {
    const entitySet = await this.metadataRepo.getEntitySetForEntity(logicalName);
    const res = await fetch(`api/data/v9.1/${entitySet}`, {
      headers: this.headers,
      body: JSON.stringify(this.sanitiseRecord(record)),
      method: 'POST',
    });

    await AuthenticatedRecordRepository.checkResponseForError(res);

    const id = res.headers.get('OData-EntityId')!.match(/\((.*)\)/)![1];
    return { entityType: logicalName, id };
  }

  /** @inheritdoc */
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

  /** @inheritdoc */
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

  /** @inheritdoc */
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
        body: JSON.stringify(this.sanitiseRecord(record)),
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
