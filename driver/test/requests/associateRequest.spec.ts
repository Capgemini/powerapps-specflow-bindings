import AssociateRequest from '../../src/requests/associateRequest';

let associateRequest: AssociateRequest;

describe('AssociateRequest', () => {
  const target: Xrm.LookupValue = {
    entityType: 'contact',
    id: '<contact-id>',
  };
  const relatedEntities: Xrm.LookupValue[] = [
    {
      entityType: 'account',
      id: '<account-id>',
    },
    {
      entityType: 'account',
      id: '<account1-id>',
    },
  ];
  const relationship = 'contact_account';

  beforeEach(() => {
    associateRequest = new AssociateRequest(target, relatedEntities, relationship);
  });

  describe('getMetadata', () => {
    it('replaces parameterTypes.target.typeName with target entity type', () => {
      expect(associateRequest.getMetadata().parameterTypes.target.typeName).toBe(`mscrm.${target.entityType}`);
    });
  });
});
