import Record from '../data/record';

export default interface RecordRepository {
  retrieveRecord(logicalName: string, id:string, query?: string): Promise<any>;
  retrieveMultipleRecords(logicalName: string, query?: string): Promise<Xrm.RetrieveMultipleResult>;
  createRecord(logicalName: string, record: Record): Promise<Xrm.LookupValue>;
  updateRecord(logicalName: string, recordId: string, record: Record): Promise<Xrm.LookupValue>;
  upsertRecord(logicalName: string, record: Record): Promise<Xrm.LookupValue>;
  deleteRecord(ref: Xrm.LookupValue): Promise<Xrm.LookupValue>;
  associateManyToManyRecords(
    primaryRecord: Xrm.LookupValue,
    relatedRecords: Xrm.LookupValue[],
    relationship: string,
  ): Promise<void>;
}
