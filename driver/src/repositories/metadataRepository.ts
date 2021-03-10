/* eslint-disable class-methods-use-this */

/**
 * Repository to handle requests for metadata.
 *
 * @export
 * @class MetadataRepository
 * @extends {Repository}
 */
export default class MetadataRepository {
  private static readonly RelationshipMetadataSet = 'RelationshipDefinitions';

  private static readonly EntityMetadataSet = 'EntityDefinitions';

  private static readonly OneToNMetadataSet = 'RelationshipDefinitions/Microsoft.Dynamics.CRM.OneToManyRelationshipMetadata';

  /**
     * Gets the entity set for an entity.
     *
     * @param {string} logicalName An entity to retrieve the entity set for.
     * @returns {Promise<string>} An entity set for the entity.
     * @memberof MetadataRepository
     */
  public async getEntitySetForEntity(logicalName: string): Promise<string> {
    const response = await fetch(
      'api/data/v9.1/'
      + `${MetadataRepository.EntityMetadataSet}?$filter=LogicalName eq '${logicalName}'&$select=EntitySetName`,
      { cache: 'force-cache' },
    );
    const result = await response.json();

    return result.value[0].EntitySetName as string;
  }

  /**
     * Gets the related entity for a navigation property.
     *
     * @param {string} logicalName The name of the entity containing the navigation property.
     * @param {string} navigationProperty The logical name of the navigation property.
     * @returns {Promise<string>} A promise which contains the logical name of the related entity.
     * @memberof MetadataRepository
     */
  public async getTargetsForLookupProperty(logicalName: string, navigationProperty: string)
    : Promise<string[] | null> {
    const response = await fetch(
      'api/data/v9.1/'
      + `${MetadataRepository.EntityMetadataSet}(LogicalName='${logicalName}')/Attributes/Microsoft.Dynamics.CRM.LookupAttributeMetadata?$filter=LogicalName eq '${navigationProperty.toLowerCase()}'&$select=Targets`,
      { cache: 'force-cache' },
    );
    const result = await response.json();

    return result.value[0] ? result.value[0].Targets : null;
  }

  /**
     * Gets the N:1 navigation property name from a 1:N navigation property,
     *
     * @param {string} navPropName A N:1 navigation property name
     * @returns {Promise<string>} A 1:N navigation property name
     * @memberof MetadataRepository
     */
  public async getLookupPropertyForCollectionProperty(navPropName: string): Promise<string> {
    const response = await fetch(
      'api/data/v9.1/'
      + `${MetadataRepository.OneToNMetadataSet}?$filter=ReferencedEntityNavigationPropertyName eq '${navPropName}'&$select=ReferencingEntityNavigationPropertyName`,
      { cache: 'force-cache' },
    );
    const result = await response.json();

    return result.value[0].ReferencingEntityNavigationPropertyName;
  }

  /**
     * Retrieves the relationship metadata for the supplied relationship name.
     * @param {string} relationshipSchemaName The schema name of the relationship.
     * @returns {Xrm.Metadata.RelationshipMetadata} The relationship metadata object.
     */
  public async getRelationshipMetadata(relationshipSchemaName: string)
    : Promise<Xrm.Metadata.OneToNRelationshipMetadata | Xrm.Metadata.NToNRelationshipMetadata> {
    const response = await fetch(
      'api/data/v9.1/'
      + `${MetadataRepository.RelationshipMetadataSet}(SchemaName='${relationshipSchemaName}')`,
      { cache: 'force-cache' },
    );

    return response.json();
  }
}
