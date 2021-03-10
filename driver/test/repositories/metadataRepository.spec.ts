import { MetadataRepository } from '../../src/repositories';

const fetchMock = require('fetch-mock/es5/client');

describe('MetadataRepository', () => {
  let metadataRepo: MetadataRepository;

  beforeEach(() => {
    metadataRepo = new MetadataRepository();
  });

  afterEach(() => {
    fetchMock.restore();
  });

  describe('getEntitySetForEntity(logicalName)', () => {
    it('returns the entity set from the entity metadata', () => {
      const entityLogicalName = 'contact';
      const entitySetName = 'contacts';
      const mockResponse = { value: [{ EntitySetName: 'contacts' }] };
      fetchMock.mock('*', { body: mockResponse, sendAsJson: true });

      expectAsync(metadataRepo.getEntitySetForEntity(entityLogicalName))
        .toBeResolvedTo(entitySetName);
    });
  });

  describe('getTargetsForLookupProperty(logicalName, navigationProperty)', () => {
    const entityLogicalName = 'contact';
    const targetEntityName = 'account';

    it('returns the target entity for a lookup attribute', () => {
      const navigationProperty = 'accountid';
      const mockResponse = { value: [{ Targets: [targetEntityName] }] };
      fetchMock.mock('*', { body: mockResponse, sendAsJson: true });

      expectAsync(metadataRepo.getTargetsForLookupProperty(entityLogicalName, navigationProperty))
        .toBeResolvedTo(mockResponse.value[0].Targets);
    });

    it('returns null if the attribute is not found', () => {
      const navigationProperty = 'accountid';
      const mockResponse = { value: [] };
      fetchMock.mock('*', { body: mockResponse, sendAsJson: true });

      expectAsync(metadataRepo.getTargetsForLookupProperty(entityLogicalName, navigationProperty))
        .toBeResolvedTo(null);
    });
  });

  describe('getLookupPropertyForCollectionProperty(navPropName)', () => {
    it('returns the lookup navigation property for a collection navigation property', () => {
      const navProp = 'contact_accounts';
      const lookupNavProp = 'contactid';
      const mockResponse = { value: [{ ReferencingEntityNavigationPropertyName: lookupNavProp }] };
      fetchMock.mock('*', { body: mockResponse, sendAsJson: true });

      expectAsync(metadataRepo.getLookupPropertyForCollectionProperty(navProp))
        .toBeResolvedTo(lookupNavProp);
    });
  });

  describe('getRelationshipMetadata(relationshipSchemaName)', () => {
    it('returns relationship metadata for the provided relationship schema name', async () => {
      const result = {};
      const relationshipSchemaName = 'contact_accounts';
      fetchMock.mock('*', { body: result, sendAsJson: true });

      expectAsync(metadataRepo.getRelationshipMetadata(relationshipSchemaName))
        .toBeResolvedTo(result as never);
    });
  });
});
