export interface DeepInsertResponse {
  record: { reference: Xrm.LookupValue, alias?: string };
  associatedRecords: { reference: Xrm.LookupValue; alias?: string }[]
}
