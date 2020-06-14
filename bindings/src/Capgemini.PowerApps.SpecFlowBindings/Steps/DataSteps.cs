namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using Capgemini.PowerApps.SpecFlowBindings;
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
        public void GivenIHaveOpened(string alias)
        {
            TestDriver.OpenTestRecord(alias);
        }

        /// <summary>
        /// Creates a test record.
        /// </summary>
        /// <param name="fileName">The name of the file containing the test record.</param>
        [Given(@"I have created '(.*)'")]
        public void GivenIHaveCreated(string fileName)
        {
            TestDriver.LoadTestData(TestDataRepository.GetTestData(fileName));
        }
    }
}
