namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using System.Configuration;
    using Capgemini.PowerApps.SpecFlowBindings;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using Microsoft.Identity.Client;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Test setup step bindings for data.
    /// </summary>
    [Binding]
    public class DataSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Opens a test record.
        /// </summary>
        /// <param name="alias">The alias of the test record.</param>
        [Given(@"I have opened '(.*)'")]
        public static void GivenIHaveOpened(string alias)
        {
            TestDriver.OpenTestRecord(alias);

            Driver.WaitForTransaction();
        }

        /// <summary>
        /// Creates a test record.
        /// </summary>
        /// <param name="fileName">The name of the file containing the test record.</param>
        [Given(@"I have created '(.*)'")]
        public static void GivenIHaveCreated(string fileName)
        {
            TestDriver.LoadTestData(TestDataRepository.GetTestData(fileName));
        }

        /// <summary>
        /// Creates a test record as a given user.
        /// </summary>
        /// <param name="alias">The user alias.</param>
        /// <param name="fileName">The name of the file containing the test record.</param>
        [Given(@"'(.*)' has created '(.*)'")]
        public static void GivenIHaveCreated(string alias, string fileName)
        {
            TestDriver.LoadTestDataAsUser(
                TestDataRepository.GetTestData(fileName),
                TestConfig.GetUser(alias).Username,
                AccessToken);
        }
    }
}
