namespace Capgemini.Dynamics.Testing.Data {
    export interface IDeepInsertResponse {
        record: {reference: Xrm.LookupValue, alias?: string};
        associatedRecords: {reference: Xrm.LookupValue; alias?: string}[]
    }
}
