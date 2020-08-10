namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Steps relating to lookups.
    /// </summary>
    [Binding]
    public class LookupSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Clicks the new button in a lookup.
        /// </summary>
        [When("I click the new button in the lookup")]
        public static void WhenIClickTheNewButtonInTheLookup()
        {
            XrmApp.Lookup.New();
        }

        /// <summary>
        /// Selects a records in a lookup by index.
        /// </summary>
        /// <param name="index">The position of the record.</param>
        [When(@"I open the record at position '(\d+)' in the lookup")]
        public static void WhenIOpenTheRecordAtPositionInTheLookup(int index)
        {
            XrmApp.Lookup.OpenRecord(index);
        }

        /// <summary>
        /// Searches records in a lookup.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <param name="control">The lookup.</param>
        [When(@"I search for '(.*)' in the '(.*)' lookup")]
        public static void WhenISearchForInTheLookup(string searchCriteria, LookupItem control)
        {
            if (control is null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            /* Doesn't work for lookups on Quick Creates
             * XrmApp.Lookup.Search(control, searchCriteria); */

            var fieldContainer = Driver.WaitUntilAvailable(By.XPath(AppElements.Xpath[AppReference.Entity.TextFieldContainer].Replace("[NAME]", control.Name)));
            var input = fieldContainer.FindElement(By.TagName("input"));
            input.Click();
            input.SendKeys(searchCriteria);
            input.SendKeys(Keys.Enter);
        }

        /// <summary>
        /// Selects a related entity in a lookup.
        /// </summary>
        /// <param name="entity">The entity label.</param>
        [When(@"I select the related '(.*)' entity in the lookup")]
        public static void WhenISelectTheRelatedEntityInTheLookup(string entity)
        {
            XrmApp.Lookup.SelectRelatedEntity(entity);
        }

        /// <summary>
        /// Switches to a given view in a lookup.
        /// </summary>
        /// <param name="viewName">The name of the view.</param>
        [When(@"I switch to the '(.*)' view in the lookup")]
        public static void WhenISwitchToTheViewInTheLookup(string viewName)
        {
            XrmApp.Lookup.SwitchView(viewName);
        }

        /// <summary>
        /// Asserts that the given record names are visible in a lookup flyout.
        /// </summary>
        /// <param name="lookupName">The name of the lookup.</param>
        /// <param name="recordNames">The names of the records that should be visible.</param>
        [Then(@"I can see only the following records in the '(.*)' lookup")]
        public static void ThenICanSeeOnlyTheFollowingRecordsInTheLookup(string lookupName, Table recordNames)
        {
            if (recordNames is null)
            {
                throw new ArgumentNullException(nameof(recordNames));
            }

            if (!Driver.TryFindElement(By.CssSelector("[data-id*=SimpleLookupControlFlyout]"), out var flyout))
            {
                if (!Driver.TryFindElement(By.CssSelector("div[data-id*=LookupResultsDropdown][aria-label*=\"Lookup results\"]"), out flyout))
                {
                    throw new ElementNotVisibleException($"The flyout is not visible for the {lookupName} lookup.");
                }
            }

            new WebDriverWait(Driver, TimeSpan.FromSeconds(5))
                .Until(d => d.FindElement(By.CssSelector("ul[aria-label=\"Lookup Search Results\"] li")));
            var items = Driver
                .FindElements(By.CssSelector("ul[aria-label=\"Lookup Search Results\"] li"))
                .Select(e => e.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0]);

            items.Count().Should().Be(recordNames.Rows.Count, because: "the flyout should only contain the given records");
            foreach (var item in items)
            {
                recordNames.Rows.Should().Contain(r => r[0] == item, because: "every given records should be present in the flyout");
            }
        }
    }
}
