/// <reference path="../../dist/specflow.driver.d.ts" />
/// <reference types="@types/jasmine" />

namespace Capgemini.Dynamics.Testing.Tests {
    describe("TestDriver", () => {
        let dataManager: jasmine.SpyObj<Data.DataManager>;
        let testDriver: TestDriver;
        beforeAll(() => {
            dataManager = jasmine.createSpyObj<Data.DataManager>("DataManager", ["createData", "cleanup", "createdRecords", "createdRecordsByAlias"]);
            testDriver = new TestDriver(dataManager);
        });
        describe(".loadJsonData(json)", () => {
            it("uses the TestDataManager to create the test data", () => {
                const logicalName = "account";
                const testData = {
                    "@logicalName": logicalName,
                    name: "Sample Account",
                    primarycontactid:
                    {
                        firstname: "John",
                        lastname: "Smith",
                    },
                };
                const createDataCalls = dataManager.createData.calls.count();

                testDriver.loadTestData(JSON.stringify(testData));

                expect(dataManager.createData).toHaveBeenCalledTimes(createDataCalls + 1);
            });
        });
        describe(".deleteTestData()", () => {
            it("uses the TestDataManager to delete the test data", () => {
                dataManager.cleanup.calls.reset();
                testDriver.deleteTestData();

                expect(dataManager.cleanup.calls.count()).toBe(1);
            });
        });
        describe(".openTestRecord(alias)", () => {
            it("throws an error the record hasn't been created", () => {
                expect(() => testDriver.openTestRecord("this is not a valid alias")).toThrowError();
            });
            it("navigates to a record", async () => {
                const openForm = jasmine.createSpy("openForm");
                window.Xrm = {
                    Navigation: {
                        openForm,
                    },
                } as any;
                dataManager.createdRecordsByAlias.someAlias = { id: "<account-id>", entityType: "account" };

                testDriver.openTestRecord("someAlias");

                expect(openForm.calls.first().args[0].entityId).toBe("<account-id>");
            });
        });
    });
}
