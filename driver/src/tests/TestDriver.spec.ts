/// <reference path="../../dist/specflow.driver.d.ts" />

namespace Capgemini.Dynamics.Testing.Tests {
    describe("TestDriver", () => {
        let dataManager: jasmine.SpyObj<Data.DataManager>;
        let testDriver: TestDriver;
        beforeEach(() => {
            dataManager = jasmine.createSpyObj<Data.DataManager>("DataManager", ["createData", "cleanup", "createdRecords"]);
            testDriver = new TestDriver(dataManager);
        });
        it("listens for the specXrm.loadTestDataRequested event", () => {
            const data = JSON.stringify({
                name: "Sample Account",
            });
            const event = new CustomEvent(
                TestEvents.LoadTestDataRequested,
                {
                    detail: {
                        data,
                    },
                });

            window.top.dispatchEvent(event);

            expect(dataManager.createData.calls.count()).toBe(1);
        });
        it("listens for the specXrm.deleteTestDataRequested event", () => {
            const event = new CustomEvent(TestEvents.DeleteTestDataRequested);

            window.top.dispatchEvent(event);

            expect(dataManager.cleanup.calls.count()).toBe(1);
        });
        it("listens for the specXrm.openRecordRequested event", async () => {
            const openForm = jasmine.createSpy("openForm");
            window.Xrm = {
                Navigation: {
                    openForm,
                },
            } as any;
            dataManager.createData.and.returnValue(
                Promise.resolve({
                    entityType: "account",
                    id: "<account-id>",
                }));
            await testDriver.loadTestData(JSON.stringify({
                "@alias": "someAlias",
                "@logicalName": "account",
                "name": "Some Account",
            } as Data.ITestRecord));
            const event = new CustomEvent(TestEvents.OpenRecordRequested, { detail: { data: "someAlias" } });

            window.top.dispatchEvent(event);

            expect(openForm.calls.first().args[0].entityId).toBe("<account-id>");
        });
        describe(".loadJsonData(json)", () => {
            it("uses the TestDataManager to create the test data", () => {
                const testData = {
                    name: "Sample Account",
                    primarycontactid:
                    {
                        firstname: "John",
                        lastname: "Smith",
                    },
                };

                testDriver.loadTestData(JSON.stringify(testData));

                expect(dataManager.createData.calls.count()).toBe(1);
            });
        });
        describe(".deleteTestData()", () => {
            it("uses the TestDataManager to delete the test data", () => {
                testDriver.deleteTestData();

                expect(dataManager.cleanup.calls.count()).toBe(1);
            });
        });
        describe(".openTestRecord(alias", () => {
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
                dataManager.createData.and.returnValue(Promise.resolve({
                    entityType: "account",
                    id: "<account-id>",
                }));
                await testDriver.loadTestData(JSON.stringify({
                    "@alias": "someAlias",
                    "@logicalName": "account",
                    "name": "Some Account",
                } as Data.ITestRecord));

                testDriver.openTestRecord("someAlias");

                expect(openForm.calls.first().args[0].entityId).toBe("<account-id>");
            });
        });
    });
}
