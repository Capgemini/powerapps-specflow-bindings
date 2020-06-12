namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using TechTalk.SpecFlow;

    /// <summary>
    /// Step bindings relating to navigation.
    /// </summary>
    [Binding]
    public class NavigationSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Opens a sub-area from the navigation bar.
        /// </summary>
        /// <param name="subAreaName">The name of the sub-area.</param>
        /// <param name="areaName">The name of the area.</param>
        [Given("I am viewing the '(.*)' sub area of the '(.*)' area")]
        public static void GivenIAmViewingTheSubArea(string subAreaName, string areaName)
        {
            XrmApp.Navigation.OpenSubArea(areaName, subAreaName);
        }
    }
}
