/// <reference path="../../../dist/specflow.driver.d.ts" />
/// <reference types="jasmine-ajax" />

namespace Capgemini.Dynamics.Testing.Data.Tests {
    describe("MetadataRepository", () => {
        let webApiOnline: jasmine.SpyObj<Xrm.WebApiOnline>;
        let metadataRepository: MetadataRepository;
        beforeEach(() => {
            webApiOnline = jasmine.createSpyObj<Xrm.WebApiOnline>("WebApiOnline", ["retrieveMultipleRecords"]);
            window.Xrm = {
                Utility: {
                    getGlobalContext: jasmine.createSpy().and.returnValue({
                        getClientUrl: jasmine.createSpy().and.returnValue("url"),
                    }),
                },
            } as any;
            metadataRepository = new MetadataRepository(webApiOnline);
        });
        describe("getEntitySetForEntity(entityLogicalName)", () => {
            it("returns the EntitySetName of the entity", async () => {
                const entitySetName = "accounts";
                webApiOnline.retrieveMultipleRecords.and.returnValue(Promise.resolve({
                    entities: [
                        { EntitySetName: entitySetName } as Xrm.Metadata.EntityMetadata,
                    ],
                }) as any);

                const result = await metadataRepository.getEntitySetForEntity("account");

                expect(result).toBe(entitySetName);
            });
        });
        describe("GetEntityForLookupProperty(entityLogicalName, navigationProperty)", () => {
            beforeEach(() => {
                jasmine.Ajax.install();
            });
            afterEach(() => {
                jasmine.Ajax.uninstall();
            });
            it("returns the Target of the lookup", async () => {
                const target = "contact";
                jasmine.Ajax.stubRequest(/a/).andReturn({
                    responseText: JSON.stringify({
                        value: [
                            {
                                Targets: [
                                    target,
                                ],
                            },
                        ],
                    }),
                    status: 200,
                });

                const result = await metadataRepository.GetEntityForLookupProperty("account", "primarycontactid");

                expect(result).toBe(target);
            });
        });
        describe("GetEntityForCollectionProperty(entityLogicalName, navigationProperty)", () => {
            it("returns the ReferencingEntity for the relationship", async () => {
                const referencingEntity = "account";
                webApiOnline.retrieveMultipleRecords.and.returnValue(Promise.resolve({
                    entities: [
                        {
                            ReferencingEntity: referencingEntity,
                        },
                    ],
                }) as any);

                const entity = await metadataRepository.GetEntityForCollectionProperty("account", "opportunity_customer_accounts");

                expect(entity).toBe(referencingEntity);
            });
        });
        describe("GetLookupPropertyForCollectionProperty(navPropName)", () => {
            it("returns the ReferencingEntityNavigationPropertyName for the relationship", async () => {
                const lookupProperty = "primarycontactid";
                webApiOnline.retrieveMultipleRecords.and.returnValue(Promise.resolve({
                    entities: [
                        {
                            ReferencingEntityNavigationPropertyName: lookupProperty,
                        },
                    ],
                }) as any);

                const result = await metadataRepository.GetLookupPropertyForCollectionProperty("contact_accounts");

                expect(result).toBe(lookupProperty);
            });
        });
    });
}
