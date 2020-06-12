namespace Capgemini.PowerApps.SpecFlowBindings.Hooks
{
    using TechTalk.SpecFlow;

    /// <summary>
    /// After scenario hooks.
    /// </summary>
    [Binding]
    public class AfterScenarioHooks : PowerAppsStepDefiner
    {
        /// <summary>
        /// Deletes the test data created during the test and disposes of the browser.
        /// </summary>
        [AfterScenario]
        public static void TestCleanup()
        {
            try
            {
                TestDriver.DeleteTestData();
            }
            finally
            {
                Quit();
            }
        }
    }
}
