/// <reference path="IRecord.ts"/>
/// <reference path="core/Repository.ts"/>

namespace Capgemini.Dynamics.Testing.Data {
    /**
     * Repository to handle CRUD operations for entityies.
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
            return this.webApi.createRecord(entityLogicalName, record) as any as Xrm.LookupValue;
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
    }
}
