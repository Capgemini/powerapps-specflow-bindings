using Capgemini.Test.Xrm.Web.Core;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Web.Steps.When
{
    [Binding]
    public class DashboardSteps : XrmWebStepDefiner
    {
        [When(@"I open the ""(.*)"" dashboard")]
        public void WhenIOpenTheDashboard(string dashboard)
        {
            Browser.Dashboard.SelectDashBoard(dashboard);
        }
    }
}
