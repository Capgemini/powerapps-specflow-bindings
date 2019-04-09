using Capgemini.Test.Xrm.Web.Core;
using Microsoft.Dynamics365.UIAutomation.Api;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Web.Steps.When
{
    /// <summary>
    /// Interaction step bindings for the command bar.
    /// </summary>
    [Binding]
    public class CommandBarSteps : XrmWebStepDefiner
    {
        /// <summary>
        /// Clicks a command from the command bar.
        /// </summary>
        /// <param name="command">The name of the command to click.</param>
        [When(@"I select the ""(.*)"" command")]
        public void WhenISelectTheCommand(string command)
        {
            Browser.CommandBar.ClickCommand(command);
        }
    }
}
