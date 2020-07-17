namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
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
            XrmApp.CommandBar.ClickCommand(commandName);
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
