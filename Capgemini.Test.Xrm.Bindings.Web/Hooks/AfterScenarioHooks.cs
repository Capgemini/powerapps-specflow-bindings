using Capgemini.Test.Xrm.Bindings.Core;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Bindings.Web.Hooks
{
    [Binding]
    public class AfterScenarioHooks : XrmWebStepDefiner
    {
        [AfterScenario(Order = 0)]
        public void DisposeBrowser()
        {
            Browser.Dispose();
        }
    }
}
