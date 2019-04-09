using Capgemini.Test.Xrm.Uci;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Web.Hooks
{
    /// <summary>
    /// After scenario hooks.
    /// </summary>
    [Binding]
    public class AfterScenarioHooks : XrmUciStepDefiner
    {
        /// <summary>
        /// Deletes the test data created during the test and disposes of the browser.
        /// </summary>
        [AfterScenario]
        public void TestCleanup()
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
