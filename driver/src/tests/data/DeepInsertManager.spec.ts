/// <reference path="../../../dist/specflow.driver.d.ts" />

namespace Capgemini.Dynamics.Testing.Data.Tests {
    describe("DeepInsertParser", () => {
        let recordRepository: jasmine.SpyObj<RecordRepository>;
        let metadataRepository: jasmine.SpyObj<MetadataRepository>;
        let deepInsertParser: DeepInsertParser;
        beforeEach(() => {
            recordRepository = jasmine.createSpyObj<RecordRepository>(
                "RecordRepository",
                [
                    "createRecord",
                    "deleteRecord"
                ]);
            metadataRepository = jasmine.createSpyObj<MetadataRepository>(
                "MetadataRepository",
                [
                    "getEntitySetForEntity",
                    "getLookupPropertyForCollectionProperty",
                    "getEntityForLookupProperty",
                    "getRelationshipMetadata"
                ]);
            deepInsertParser = new DeepInsertParser(metadataRepository, recordRepository);
        });
        describe(".deepInsert(logicalName, record)", () => {
            it("creates each object in the object graph", async () => {
                metadataRepository.getLookupPropertyForCollectionProperty.and.returnValue(Promise.resolve("customerid_account"));
                metadataRepository.getRelationshipMetadata.and.returnValue(Promise.resolve({ RelationshipType: "OneToManyRelationship" }))
                recordRepository.createRecord
                    .and.returnValues(
                        Promise.resolve({ id: "<company-id>", entityType: "account" }),
                        Promise.resolve({ id: "<contact-id>", entityType: "contact" }),
                        Promise.resolve({ id: "<account-id>", entityType: "account" }),
                        Promise.resolve({ id: "<opportunity-id>", entityType: "opportunity" }));

                const testRecord: IRecord = {
                    name: "Sample Account",
                    opportunity_customer_accounts: [
                        {
                            name: "Test Opportunity associated to Sample Account",
                        },
                    ],
                    primarycontactid:
                    {
                        firstname: "John",
                        lastname: "Smith",
                        masterid: {
                            firstname: "Jim",
                            lastname: "Smith",
                        },
                    },
                };

                await deepInsertParser.deepInsert("account", testRecord);

                expect(recordRepository.createRecord.calls.count()).toBe(4);
            });
            it("replaces nested objects with @odata.bind associations", async () => {
                const entitySetName = "contacts";
                const contactId = "<contact-id>";
                metadataRepository.getEntitySetForEntity.and.returnValue(Promise.resolve(entitySetName));
                recordRepository.createRecord.and.returnValues(
                    Promise.resolve({ id: contactId, entityType: "contact" }),
                    Promise.resolve({ id: "<account-id>", entityType: "account" }));

                await deepInsertParser.deepInsert("account",
                    {
                        name: "Sample Account",
                        primarycontactid:
                        {
                            firstname: "John",
                            lastname: "Smith",
                        },
                    });

                expect(recordRepository.createRecord.calls.mostRecent().args[1]["primarycontactid@odata.bind"])
                    .toBe(`/${entitySetName}(${contactId})`);
            });
            it("replaces @alias.bind", async () => {
                let createdRecordsByAlias: { [alias: string]: Xrm.LookupValue }
                createdRecordsByAlias = {};
                const entitySetName = "contacts";
                const contactId = "<contact-id>";
                let createdRecords: Xrm.LookupValue[];
                createdRecords = [];
                metadataRepository.getEntitySetForEntity.and.returnValue(Promise.resolve(entitySetName));
                recordRepository.createRecord.and.returnValues(
                    Promise.resolve({ id: contactId, entityType: "contact" }),
                    Promise.resolve({ id: "<account-id>", entityType: "account" }));
                const deepInsertResponse = await deepInsertParser.deepInsert("contact",
                    {
                        "@alias": "a contact",
                        firstname: "John",
                        lastname: "Smith",
                    });
                const newRecords = [deepInsertResponse.record, ...deepInsertResponse.associatedRecords];
                createdRecords.push(...newRecords.map(r => r.reference));
                newRecords
                    .filter(r => r.alias)
                    .forEach(aliasedRecord => createdRecordsByAlias[aliasedRecord.alias!] = aliasedRecord.reference)
                await deepInsertParser.deepInsert("account",
                    {
                        name: "Sample Account",
                        "primarycontactid@alias.bind": "a contact"
                    }, createdRecordsByAlias);


                expect(recordRepository.createRecord.calls.mostRecent().args[1]["primarycontactid@odata.bind"])
                    .toBe(`/${entitySetName}(${contactId})`);
            });
            it("removes nested arrays from the record", async () => {
                metadataRepository.getEntitySetForEntity.and.returnValue(Promise.resolve("accounts"));
                metadataRepository.getRelationshipMetadata.and.returnValue(Promise.resolve({ RelationshipType: "OneToManyRelationship" }))
                metadataRepository.getLookupPropertyForCollectionProperty.and.returnValue(Promise.resolve("customerid_account"));
                recordRepository.createRecord.and.returnValue(Promise.resolve({
                    entityType: "account",
                    id: "accountid",
                }));

                const testRecord: IRecord = {
                    name: "Sample Account",
                    opportunity_customer_accounts: [
                        {
                            name: "Test Opportunity associated to Sample Account",
                        },
                    ],
                };

                await deepInsertParser.deepInsert("account", testRecord);

                expect(recordRepository.createRecord.calls.first().args[1].opportunity_customer_accounts).toBeUndefined();
            });
            it("creates records in nested arrays with @odata.bind associations back to the parent object", async () => {
                const navigationProperty = "customerid_account";
                const entitySet = "accounts";
                const createdRecord: Xrm.LookupValue = {
                    entityType: "account",
                    id: "accountid",
                };
                metadataRepository.getEntitySetForEntity.and.returnValue(Promise.resolve(entitySet));
                metadataRepository.getRelationshipMetadata.and.returnValue(Promise.resolve({ RelationshipType: "OneToManyRelationship" }))
                metadataRepository.getLookupPropertyForCollectionProperty.and.returnValue(Promise.resolve(navigationProperty));
                recordRepository.createRecord.and.returnValue(Promise.resolve(createdRecord));
                const testRecord: IRecord = {
                    name: "Sample Account",
                    opportunity_customer_accounts: [
                        {
                            name: "Test Opportunity associated to Sample Account",
                        },
                    ],
                };

                await deepInsertParser.deepInsert("account", testRecord);

                expect(recordRepository.createRecord.calls.mostRecent().args[1][`${navigationProperty}@odata.bind`])
                    .toBe(`/${entitySet}(${createdRecord.id})`);
            });
            it("queries for nested object's entity set name", async () => {
                metadataRepository.getEntitySetForEntity.and.returnValues(Promise.resolve("contacts"), Promise.resolve("accounts"));
                recordRepository.createRecord.and.returnValues(
                    Promise.resolve({ id: "<contact-id>", entityType: "contact" }),
                    Promise.resolve({ id: "<account-id>", entityType: "account" }));

                await deepInsertParser.deepInsert("account",
                    {
                        name: "Sample Account",
                        primarycontactid:
                        {
                            firstname: "John",
                            lastname: "Smith",
                        },
                    });

                expect(metadataRepository.getEntitySetForEntity).toHaveBeenCalledTimes(1);
            });
            it("returns an entity reference for each created record", async () => {
                const expectedEntityReference = { id: "<contact-id>", entityType: "contact" };
                const createResponses = [
                    Promise.resolve({ id: "<account-id>", entityType: "account" }),
                    Promise.resolve(expectedEntityReference)
                ];
                recordRepository.createRecord
                    .and.returnValues(...createResponses);

                const entityReference = await deepInsertParser.deepInsert("account",
                    {
                        name: "Sample Account",
                        primarycontactid:
                        {
                            firstname: "John",
                            lastname: "Smith",
                        },
                    });

                expect(entityReference.record.reference).toBe(expectedEntityReference);
            });
        });
    });
}
