namespace Capgemini.Dynamics.Testing.Data {
    export interface IDeepInsertResponse {
        record: Xrm.LookupValue;
        associatedRecords: Xrm.LookupValue[];
    }
}
