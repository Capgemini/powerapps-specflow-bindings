using Capgemini.Test.Xrm.Bindings.Core;
using System;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Bindings.Web.Hooks
{
    [Binding]
    public class BeforeTestRunHooks : XrmWebStepDefiner
    {
        [BeforeTestRun(Order = 0)]
        public static void ConfigureDriverSettings()
        {
            Browser.Driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 30);
        }
    }
}
