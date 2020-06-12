/// <reference path="./core/Repository.ts" />

namespace Capgemini.Dynamics.Testing.Data {
    /**
     * Repository to handle requests for metadata.
     *
     * @export
     * @class MetadataRepository
     * @extends {Repository}
     */
    export class MetadataRepository extends Core.Repository {
        private static readonly EntityMetadataSet = "EntityDefinitions";
        private static readonly OneToNMetadataSet = "RelationshipDefinitions/Microsoft.Dynamics.CRM.OneToManyRelationshipMetadata";

        private readonly clientUrl: string;

        /**
         * Creates an instance of MetadataRepository.
         * @memberof MetadataRepository
         */
        constructor(webApi: Xrm.WebApiOnline) {
            super(webApi);
            this.clientUrl = Xrm.Utility.getGlobalContext().getClientUrl();
        }

        /**
         * Gets the entity set for an entity.
         *
         * @param {string} entityLogicalName An entity to retrieve the entity set for.
         * @returns {Promise<string>} An entity set for the entity.
         * @memberof MetadataRepository
         */
        public async getEntitySetForEntity(entityLogicalName: string): Promise<string> {
            const response =
                await this.webApi.retrieveMultipleRecords(
                    MetadataRepository.EntityMetadataSet,
                    `$filter=LogicalName eq '${entityLogicalName}'&$select=EntitySetName`);

            return response.entities[0].EntitySetName as string;
        }

        /**
         * Gets the related entity for a navigation property.
         *
         * @param {string} entityLogicalName The logical name of the entity containing the navigation property.
         * @param {string} navigationProperty The logical name of the navigation property.
         * @returns {Promise<string>} A promise which contains the logical name of the related entity.
         * @memberof MetadataRepository
         */
        public async GetEntityForLookupProperty(entityLogicalName: string, navigationProperty: string): Promise<string> {
            return new Promise<string>((resolve) => {
                const xhr = new XMLHttpRequest();
                xhr.open("GET",
                    this.clientUrl +
                    `/api/data/v9.0/EntityDefinitions(LogicalName='${entityLogicalName}')` +
                    "/Attributes/Microsoft.Dynamics.CRM.LookupAttributeMetadata" +
                    `?$filter=LogicalName eq '${navigationProperty}'&$select=Targets`, true);
                xhr.setRequestHeader("Accept", "application/json");
                xhr.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                xhr.setRequestHeader("OData-MaxVersion", "4.0");
                xhr.setRequestHeader("OData-Version", "4.0");
                xhr.onreadystatechange = () => {
                    if (xhr.readyState === 4 && xhr.status === 200) {
                        resolve(JSON.parse(xhr.responseText).value[0].Targets[0]);
                    }
                };
                xhr.send();
            });
        }

        /**
         * Retrieves the referenced entity for a 1:N navigation property.
         *
         * @param {string} entityLogicalName The logical name of the entity which has a collection navigation property.
         * @param {string} navigationProperty The navigation property name for the collection.
         * @returns {Promise<string>} An async result which resolves to the entity name for collection.
         * @memberof MetadataRepository
         */
        public async GetEntityForCollectionProperty(entityLogicalName: string, navigationProperty: string): Promise<string> {
            const response = await this.webApi.retrieveMultipleRecords(
                MetadataRepository.OneToNMetadataSet,
                `$filter=ReferencedEntity eq '${entityLogicalName}' and ReferencedEntityNavigationPropertyName eq '${navigationProperty}'` +
                `&$select=ReferencingEntity`,
            );
            return response.entities[0].ReferencingEntity as string;
        }

        /**
         * Gets the N:1 navigation property name from a 1:N navigation property,
         *
         * @param {string} navPropName A N:1 navigation property name
         * @returns {Promise<string>} A 1:N navigation property name
         * @memberof MetadataRepository
         */
        public async GetLookupPropertyForCollectionProperty(navPropName: string): Promise<string> {
            const response = await this.webApi.retrieveMultipleRecords(
                MetadataRepository.OneToNMetadataSet,
                `$filter=ReferencedEntityNavigationPropertyName eq '${navPropName}'` +
                "&$select=ReferencingEntityNavigationPropertyName",
            );
            return response.entities[0].ReferencingEntityNavigationPropertyName as string;
        }
    }
}
