namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using TechTalk.SpecFlow;

    /// <summary>
    /// Step bindings related to dialogs.
    /// </summary>
    [Binding]
    public class DialogSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Clicks the confirmation button on a confirm dialog.
        /// </summary>
        [When(@"I confirm when presented with the confirmation dialog")]
        public void WhenIConfirmWhenPresentedWithTheConfirmationDialog()
        {
            XrmApp.Dialogs.ConfirmationDialog(true);
            XrmApp.ThinkTime(2000);
        }
    }
}
