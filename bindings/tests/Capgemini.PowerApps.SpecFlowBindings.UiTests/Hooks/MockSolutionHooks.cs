using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow;

namespace Capgemini.PowerApps.SpecFlowBindings.UiTests.Hooks
{
    /// <summary>
    /// Hooks related to the mock solution used for testing.
    /// </summary>
    [Binding]
    public class MockSolutionHooks : PowerAppsStepDefiner
    {
        private const string SolutionName = "sb_PowerAppsSpecFlowBindings_Mock";

        private static string SolutionPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{SolutionName}.zip");

        /// <summary>
        /// Installs the mock solution to test against.
        /// If the solution is already installed in the target environment, you may wish to comment out this binding if you're running the tests locally to save time.
        /// </summary>
        [BeforeTestRun]
        public static void ImportMockSolution()
        {
            using (var serviceClient = GetServiceClient())
            {
                serviceClient.ImportSolutionToCrm(SolutionPath, out var importId);

                if (serviceClient.LastCrmException != null)
                {
                    throw serviceClient.LastCrmException;
                }
            }
        }

        /// <summary>
        /// Uninstalls the mock solution. 
        /// You may wish to comment out this binding if you're running the tests locally to save time.
        /// </summary>
        [AfterTestRun]
        public static void RemoveMockSolution()
        {
            using (var serviceClient = GetServiceClient())
            {
                var query = new QueryByAttribute("solution");
                query.AddAttributeValue("uniquename", SolutionName);
                query.ColumnSet = new ColumnSet(false);

                var solutionId = serviceClient.RetrieveMultiple(query).Entities.Select(e => e.Id).FirstOrDefault();
                if (solutionId == default)
                {
                    return;
                }

                serviceClient.Delete("solution", solutionId);

                if (serviceClient.LastCrmException != null)
                {
                    throw serviceClient.LastCrmException;
                }
            }
        }

        private static CrmServiceClient GetServiceClient()
        {
            var admin = TestConfig.GetUser("an admin");

            return new CrmServiceClient(
                $"AuthType=OAuth; " +
                $"Username={admin.Username}; " +
                $"Password={admin.Password}; " +
                $"Url={TestConfig.GetTestUrl()}; " +
                $"AppId=51f81489-12ee-4a9e-aaae-a2591f45987d; " +
                $"RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97; " +
                $"LoginPrompt=Auto");
        }
    }
}
