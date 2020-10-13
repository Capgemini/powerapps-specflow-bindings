/* eslint-disable @typescript-eslint/no-unused-vars */
declare namespace Xrm {
  namespace Metadata {
    type RelationshipMetadata =
      Xrm.Metadata.NToNRelationshipMetadata | Xrm.Metadata.OneToNRelationshipMetadata;

    interface NToNRelationshipMetadata {
      Entity1LogicalName: string;
      Entity2LogicalName: string;
      IntersectEntityName: string;
      Entity1IntersectAttribute: string;
      Entity2IntersectAttribute: string;
      Entity1NavigationPropertyName: string;
      Entity2NavigationPropertyName: string;
      IsCustomRelationship: boolean;
      IsValidForAdvancedFind: boolean;
      SchemaName: string;
      SecurityTypes: string;
      IsManaged: boolean;
      RelationshipType: string;
      IntroducedVersion: string;
      MetadataId: string;
    }

    interface OneToNRelationshipMetadata {
      ReferencedAttribute: string;
      ReferencedEntity: string;
      ReferencingAttribute: string;
      ReferencingEntity: string;
      IsHierarchical: boolean;
      ReferencedEntityNavigationPropertyName: string;
      ReferencingEntityNavigationPropertyName: string;
      RelationshipBehavior: number;
      IsCustomRelationship: boolean;
      IsValidForAdvancedFind: boolean;
      SchemaName: string;
      SecurityTypes: string;
      IsManaged: boolean;
      RelationshipType: string;
      IntroducedVersion: string;
      MetadataId: string;
    }

    interface ActionRequestMetadata {
      parameterTypes: { [parameter: string]: ParameterMetadata },
      operationType: number,
      operationName: string
    }

    interface ParameterMetadata {
      typeName: string,
      structuralProperty: number
    }
  }
}
