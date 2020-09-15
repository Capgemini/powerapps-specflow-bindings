/// <reference path="../../../dist/specflow.driver.d.ts" />

namespace Capgemini.Dynamics.Testing.Data.Tests {
    describe("RecordRepository", () => {
        let xrmWebApi: jasmine.SpyObj<Xrm.WebApiOnline>;
        let recordRepository: RecordRepository;
        beforeEach(() => {
            xrmWebApi = jasmine.createSpyObj<Xrm.WebApiOnline>("XrmWebApi", ["createRecord", "deleteRecord"]);
            recordRepository = new RecordRepository(xrmWebApi);
        });
        describe("createRecord(entityLogicalName, record)", () => {
            it("returns a reference to the created record", async () => {
                const entityReference: Xrm.CreateResponse = {
                    entityType: "account",
                    id: "<account-id>",
                };
                xrmWebApi.createRecord.and.returnValue(Promise.resolve(entityReference) as any);

                const result = await recordRepository.createRecord("account", {
                    name: "Test Account",
                });

                expect(result).toBe(entityReference);
            });
        });
        describe("deleteRecord(entityReference)", () => {
            it("returns a reference to the deleted record", async () => {
                const entityReference: Xrm.LookupValue = {
                    entityType: "account",
                    id: "<account-id>",
                };
                xrmWebApi.deleteRecord.and.returnValue(Promise.resolve(entityReference) as any);

                const result = await recordRepository.deleteRecord(entityReference);

                expect(result).toBe(entityReference);
            });
        });
    });
}
