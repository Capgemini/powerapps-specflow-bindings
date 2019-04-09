using Capgemini.Test.Xrm.Web.Core;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Web.Steps.When
{
    /// <summary>
    /// Interaction step bindings for dashboards.
    /// </summary>
    [Binding]
    public class DashboardSteps : XrmWebStepDefiner
    {
        /// <summary>
        /// Selects a dashboard.
        /// </summary>
        /// <param name="dashboard">The name of the dashboard to select.</param>
        [When(@"I open the ""(.*)"" dashboard")]
        public void WhenIOpenTheDashboard(string dashboard)
        {
            Browser.Dashboard.SelectDashBoard(dashboard);
        }
    }
}
