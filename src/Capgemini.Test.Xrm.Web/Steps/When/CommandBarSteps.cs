using Capgemini.Test.Xrm.Web.Core;
using Microsoft.Dynamics365.UIAutomation.Api;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Web.Steps.When
{
    [Binding]
    public class CommandBarSteps : XrmWebStepDefiner
    {
        [When(@"I select the ""(.*)"" command")]
        public void WhenISelectTheCommand(string command)
        {
            Browser.CommandBar.ClickCommand(command);
        }
    }
}
