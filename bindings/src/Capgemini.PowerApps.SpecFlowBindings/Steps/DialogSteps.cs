namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using Reqnroll;

    /// <summary>
    /// Step bindings related to dialogs.
    /// </summary>
    [Binding]
    public class DialogSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Clicks the confirmation button on a confirm dialog.
        /// </summary>
        /// <param name="option">The option to click.</param>
        [When(@"^I (confirm|cancel) when presented with the confirmation dialog$")]
        public static void WhenIConfirmWhenPresentedWithTheConfirmationDialog(string option)
        {
            XrmApp.Dialogs.ConfirmationDialog(option == "confirm");
            XrmApp.ThinkTime(2000);
        }

        /// <summary>
        /// Assigns to the current user.
        /// </summary>
        [When("I assign to me on the assign dialog")]
        public static void WhenIAssignToMeOnTheAssignDialog()
        {
            XrmApp.Dialogs.Assign(Dialogs.AssignTo.Me);
        }

        /// <summary>
        /// Assigns to a user or team with the given name.
        /// </summary>
        /// <param name="assignTo">User or team.</param>
        /// <param name="userName">The name of the user or team.</param>
        [When("I assign to a (user|team) named '(.*)' on the assign dialog")]
        public static void WhenIAssignToANamedOnTheAssignDialog(Dialogs.AssignTo assignTo, string userName)
        {
            XrmApp.Dialogs.Assign(assignTo, userName);
        }

        /// <summary>
        /// Closes an opportunity.
        /// </summary>
        /// <param name="status">Whether the opportunity was won.</param>
        [When("I close the opportunity as (won|lost)")]
        public static void WhenICloseTheOpportunityAs(string status)
        {
            XrmApp.Dialogs.CloseOpportunity(status == "won");
        }

        /// <summary>
        /// Closes a warning dialog.
        /// </summary>
        [When("I close the warning dialog")]
        public static void WhenICloseTheWarningDialog()
        {
            XrmApp.Dialogs.CloseWarningDialog();
        }

        /// <summary>
        /// Clicks an option on the publish dialog.
        /// </summary>
        /// <param name="option">The option to click.</param>
        [When("I click (confirm|cancel) on the publish dialog")]
        public static void WhenIClickOnThePublishDialog(string option)
        {
            XrmApp.Dialogs.PublishDialog(option == "confirm");
        }

        /// <summary>
        /// Clicks an option on the set state dialog.
        /// </summary>
        /// <param name="option">The option to click.</param>
        [When("^I click (ok|cancel) on the set state dialog$")]
        public static void WhenIClickOnTheSetStateDialog(string option)
        {
            XrmApp.Dialogs.SetStateDialog(option == "ok");
        }
    }
}
