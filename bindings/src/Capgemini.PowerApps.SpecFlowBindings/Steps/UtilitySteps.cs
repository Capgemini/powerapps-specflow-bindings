namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using Capgemini.PowerApps.SpecFlowBindings;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Steps providing various utilities.
    /// </summary>
    [Binding]
    public class UtilitySteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Waits for a given number of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds to wait.</param>
        [When(@"I wait up to '(.*)' seconds")]
        public static void WhenIWaitUpToSeconds(int seconds)
        {
            XrmApp.ThinkTime(seconds * 1000);
        }
    }
}
