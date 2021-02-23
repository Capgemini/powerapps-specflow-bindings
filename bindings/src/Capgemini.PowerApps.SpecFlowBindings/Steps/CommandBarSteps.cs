namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using Capgemini.PowerApps.SpecFlowBindings.Extensions;
    using FluentAssertions;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Step bindings relating to the command bar.
    /// </summary>
    [Binding]
    public class CommandBarSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Selects a command with the given label.
        /// </summary>
        /// <param name="commandName">The label of the command.</param>
        [When("I select the '(.*)' command")]
        public static void WhenISelectTheCommand(string commandName)
        {
            // TODO: Replace with commented out code when new EasyRepro version available.
            // XrmApp.CommandBar.ClickCommand(commandName);
            Client.ClickCommandV2(commandName);
        }

        /// <summary>
        /// Selects a command under a flyout with the given label.
        /// </summary>
        /// <param name="commandName">The label of the command.</param>
        /// <param name="flyoutName">The label of the flyout.</param>
        [When("I select the '([^']+)' command under the '([^']+)' flyout")]
        public static void WhenISelectTheCommandUnderTheFlyout(string commandName, string flyoutName)
        {
            // TODO: Replace with commented out code when new EasyRepro version available.
            // XrmApp.CommandBar.ClickCommand(flyoutName, commandName);
            Client.ClickCommandV2(flyoutName, commandName);
        }

        /// <summary>
        /// Asserts that a command is available in the command bar.
        /// </summary>
        /// <param name="commandName">The label of the command.</param>
        [Then("I can see the '(.*)' command")]
        public static void ThenICanSeeTheCommand(string commandName)
        {
            XrmApp.CommandBar.GetCommandValues(true).Value.Should().Contain(commandName);
        }

        /// <summary>
        /// Asserts that a command is available in the command bar.
        /// </summary>
        /// <param name="commandName">The label of the command.</param>
        [Then("I can not see the '(.*)' command")]
        public static void ThenICanNotSeeTheCommand(string commandName)
        {
            XrmApp.CommandBar.GetCommandValues(true).Value.Should().NotContain(commandName);
        }
    }
}
