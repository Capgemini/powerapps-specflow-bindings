import { MetadataRepository, RecordRepository } from '../repositories';

/**
 * Parses deep insert objects and returns references to all created records.
 *
 * @export
 * @class RecordService
 */
export default class RecordService {
  private readonly recordRepository: RecordRepository;

  private readonly metadataRepository: MetadataRepository;

  /**
       * Creates an instance of RecordService.
       * @param {MetadataRepository} metadataRepository A metadata repository.
       * @param {RecordRepository} recordRepository A record repository.
       * @memberof RecordService
       */
  constructor(metadataRepository: MetadataRepository, recordRepository: RecordRepository) {
    this.metadataRepository = metadataRepository;
    this.recordRepository = recordRepository;
  }

  /**
       * A deep insert which returns a reference to all created records.
       *
       * @param {string} logicalName The entity logical name of the root record.
       * @param {Record} record The deep insert object.
       * @param dataByAlias References to previously created records by alias.
       * @param {RecordRepository} repository An optional repository to override the default.
       * @returns {Promise<DeepInsertResponse>} An async result with references to created records.
       * @memberof DeepInsertService
       */
  public async getExistingRecord(
    logicalName: string,
    id: string,
    metadataRepository?: MetadataRepository,
    recordRepository?: RecordRepository,
  ): Promise<Xrm.LookupValue> {
    const metadataRepo = metadataRepository ?? this.metadataRepository;
    const recordRepo = recordRepository ?? this.recordRepository;
    const primaryColumnName = await metadataRepo.getPrimaryColumnName(logicalName);
    const result = await recordRepo.retrieveRecord(logicalName, id, `?$select=${primaryColumnName}`);
    const reference: Xrm.LookupValue = { id, name: result?.[primaryColumnName] ?? '', entityType: logicalName };
    return reference;
  }
}
