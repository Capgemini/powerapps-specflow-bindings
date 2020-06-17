/// <reference path="../../../dist/specflow.driver.d.ts" />

namespace Capgemini.Dynamics.Testing.Data.Tests {
    describe("TestDriver", () => {
        let recordRepository: jasmine.SpyObj<RecordRepository>;
        let deepInsertParser: jasmine.SpyObj<DeepInsertParser>;
        let dataManager: DataManager;
        beforeEach(() => {
            recordRepository = jasmine.createSpyObj<RecordRepository>(
                "RecordRepository", ["createRecord", "deleteRecord"]);
            deepInsertParser = jasmine.createSpyObj<DeepInsertParser>("DeepInsertParser", ["deepInsert"]);
            dataManager = new DataManager(recordRepository, deepInsertParser);
        });
        describe(".createData(record)", () => {
            it("tracks all created records", async () => {
                const associatedRecords: { alias?: string, reference: Xrm.LookupValue }[] = [{
                    reference: {
                        entityType: "opportunity",
                        id: "<opportunity-id>",
                    }
                }];
                const record: { alias?: string, reference: Xrm.LookupValue } = {
                    reference: {
                        entityType: "account",
                        id: "<account-id>",
                    }
                };
                deepInsertParser.deepInsert.and.returnValue(Promise.resolve({
                    associatedRecords,
                    record,
                }));

                await dataManager.createData("account",
                    {
                        name: "Sample Account",
                        primarycontactid:
                        {
                            firstname: "John",
                            lastname: "Smith",
                        },
                    });

                expect(dataManager.createdRecords).toEqual([record.reference, ...associatedRecords.map(r => r.reference)]);
            });
        });
        describe(".cleanup()", () => {
            it("deletes the root record", async () => {
                deepInsertParser.deepInsert.and.returnValue(Promise.resolve({
                    associatedRecords: [],
                    record: {
                        reference: {
                            entityType: "account",
                            id: "<account-id>",
                        }
                    },
                }));
                await dataManager.createData("account",
                    {
                        name: "Sample Account",
                    });

                await dataManager.cleanup();

                expect(recordRepository.deleteRecord.calls.count()).toBe(1);
            });
            it("deletes associated records", async () => {
                deepInsertParser.deepInsert.and.returnValue(Promise.resolve({
                    associatedRecords: [{
                        reference: {
                            entityType: "opportunity",
                            id: "<opportunity-id>",
                        }
                    }],
                    record: {
                        reference: {
                            entityType: "account",
                            id: "<account-id>",
                        }
                    },
                }));
                await dataManager.createData("account",
                    {
                        name: "Sample Account",
                        primarycontactid:
                        {
                            firstname: "John",
                            lastname: "Smith",
                        },
                    });

                await dataManager.cleanup();

                expect(recordRepository.deleteRecord.calls.count()).toBe(2);
            });
            it("does not attempt to delete previously deleted records", async () => {
                deepInsertParser.deepInsert.and.returnValue(Promise.resolve({
                    associatedRecords: [],
                    record: {
                        reference: {
                            entityType: "account",
                            id: "<account-id>",
                        }
                    },
                }));
                await dataManager.createData("account",
                    {
                        name: "Sample Account",
                    });

                await dataManager.cleanup();
                await dataManager.cleanup();

                expect(recordRepository.deleteRecord.calls.count()).toBe(1);
            });
            it("retries failed delete requests", async () => {
                deepInsertParser.deepInsert.and.returnValue(Promise.resolve({
                    associatedRecords: [],
                    record: {
                        reference: {
                            entityType: "account",
                            id: "<account-id>",
                        }
                    },
                }));
                await dataManager.createData("account",
                    {
                        name: "Sample Account",
                    });
                recordRepository.deleteRecord.and.throwError("Failed to delete");

                dataManager.cleanup();

                expect(recordRepository.deleteRecord.calls.count()).toBe(3);
            });
        });
    });
}
