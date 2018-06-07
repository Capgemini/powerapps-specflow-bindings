using Capgemini.Test.Xrm.Bindings.Core;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Bindings.Web.Hooks
{
    [Binding]
    public class AfterScenarioHooks : XrmWebStepDefiner
    {
        [AfterScenario(Order = 0)]
        [Scope(Tag = "disposeBrowser")]
        public void DisposeBrowser()
        {
            Browser.Dispose();
        }
    }
}
