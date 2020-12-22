namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Tooling.Connector;
    using System.ServiceModel;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Steps relating to configuring the test environment.
    /// </summary>
    [Binding]
    public class TestEnvironmentConfigSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Enables categorized search for the given entity.
        /// </summary>
        /// <param name="entityLogicalName">The name of the entity.</param>
        [Given("the '(.*)' entity is enabled for categorized search")]
        public static void GivenTheEntityIsEnabledForCategorizedSearch(string entityLogicalName)
        {
            using (var serviceClient = GetServiceClient())
            {
                var request = new OrganizationRequest("SaveEntityGroupConfiguration");
                request.Parameters["EntityGroupName"] = "Mobile Client Search";
                request.Parameters["EntityGroupConfiguration"] = new QuickFindConfigurationCollection { new QuickFindConfiguration(entityLogicalName) };

                try
                {
                    serviceClient.Execute(request);
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    // Invalid search entity
                    if (ex.Detail.ErrorCode == -2147089919)
                    {
                        // Already configured as a search entity
                        return;
                    }
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
