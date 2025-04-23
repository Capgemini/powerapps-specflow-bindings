namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using Reqnroll;

    /// <summary>
    /// Steps relating to dashboards.
    /// </summary>
    [Binding]
    public class DashboardSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Selects a dashboard.
        /// </summary>
        /// <param name="dashboardName">Dashboard name.</param>
        [When("I select the '(.*)' dashboard")]
        public static void WhenISelectTheDashboard(string dashboardName)
        {
            XrmApp.Dashboard.SelectDashboard(dashboardName);
        }
    }
}
