namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using TechTalk.SpecFlow;

    /// <summary>
    /// Step bindings relating to the command bar.
    /// </summary>
    [Binding]
    public class CommandSteps : PowerAppsStepDefiner
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
    }
}
