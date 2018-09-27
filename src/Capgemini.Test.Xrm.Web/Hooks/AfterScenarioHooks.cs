using Capgemini.Test.Xrm.Web.Core;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Web.Hooks
{
    [Binding]
    public class AfterScenarioHooks : XrmWebStepDefiner
    {
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
