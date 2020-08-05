/// <reference path="IRecord.ts"/>
/// <reference path="core/Repository.ts"/>
/// <reference path="../requests/AssociateRequest.ts"/>

namespace Capgemini.Dynamics.Testing.Data {
    /**
     * Repository to handle CRUD operations for entities.
     *
     * @export
     * @class RecordRepository
     * @extends {Repository}
     */
    export class RecordRepository extends Core.Repository {
        /**
         * Creates an entity record.
         *
         * @param {string} entityLogicalName A logical name for the entity to create.
         * @param {IRecord} record A record to create.
         * @returns {Xrm.LookupValue} An entity reference to the created entity.
         * @memberof RecordRepository
         */
        public async createRecord(entityLogicalName: string, record: IRecord): Promise<Xrm.LookupValue> {
            if (record["@key"]) {
                return this.upsertRecord(entityLogicalName, record);
            }
            else {
                return this.webApi.createRecord(entityLogicalName, record) as any as Xrm.LookupValue;
            }
        }

        /**
         * Upserts an entity record.
         * @param {string} entityLogicalName A logical name for the entity to upsert.
         * @param {IRecord} record A record to upsert.
         * @returns {Xrm.LookupValue} An entity reference to the upserted entity.
         * @memberof RecordRepository
         */
        private async upsertRecord(entityLogicalName: string, record: IRecord): Promise<Xrm.LookupValue> {
            const retrieveResponse = await this.webApi.retrieveMultipleRecords(
                entityLogicalName,
                `?$filter=${record["@key"]} eq '${record[record["@key"] as string]}'
                    &$select=${entityLogicalName}id`
            );

            if (retrieveResponse.entities.length > 0) {
                const id = retrieveResponse.entities[0][`${entityLogicalName}id`];

                await this.webApi.updateRecord(entityLogicalName, id, record);

                return {
                    entityType: entityLogicalName,
                    id
                };
            }
            else {
                return this.webApi.createRecord(entityLogicalName, record);
            }
        }

        /**
         * Deletes an entity record.
         *
         * @param {Xrm.LookupValue} entityReference A reference to the entity to delete.
         * @returns {Xrm.LookupValue} A reference to the deleted entity.
         * @memberof RecordRepository
         */
        public async deleteRecord(entityReference: Xrm.LookupValue): Promise<Xrm.LookupValue> {
            return this.webApi.deleteRecord(entityReference.entityType, entityReference.id) as any as Xrm.LookupValue;
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
            relationshipName: string
        ): Promise<Xrm.ExecuteResponse> {
            return await this.webApi.execute(new Requests.AssociateRequest(
                primaryRecord,
                relatedRecords,
                relationshipName
            ));
        }
    }
}
