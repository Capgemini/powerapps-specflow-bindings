namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using OpenQA.Selenium;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Step bindings related to lookup dialogs.
    /// </summary>
    [Binding]
    public class LookupDialogSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Searches and Selects the first matching result.
        /// </summary>
        /// <param name="searchTerm">The term to search for.</param>
        [When("I select '([^']+)' in the lookup dialog")]
        public static void WhenISelectInTheLookupDialog(string searchTerm)
        {
            var fieldContainer = Driver.WaitUntilAvailable(By.CssSelector("div[id=\"lookupDialogContainer\"] div[id=\"lookupDialogLookup\"]"));
            var input = fieldContainer.FindElement(By.TagName("input"));
            input.Click();
            input.SendKeys(searchTerm);
            input.SendKeys(Keys.Enter);

            fieldContainer.WaitUntilAvailable(By.CssSelector("li[data-id*=\"LookupResultsPopup\"]"))
                .Click();
        }

        /// <summary>
        /// Clicks the Add button.
        /// </summary>
        [When("I click Add in the lookup dialog")]
        public static void WhenIClickAddInTheLookupDialog()
        {
            var container = Driver.WaitUntilAvailable(By.CssSelector("div[id=\"lookupDialogContainer\"]"));

            container.FindElement(By.CssSelector("button[data-id*=\"lookupDialogSaveBtn\"]"))
                .Click();
        }
    }
}
