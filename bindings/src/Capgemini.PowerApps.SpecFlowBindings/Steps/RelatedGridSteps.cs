namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using Capgemini.PowerApps.SpecFlowBindings;
    using FluentAssertions;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using OpenQA.Selenium;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Steps relating to related grids.
    /// </summary>
    [Binding]
    public class RelatedGridSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Selects a records in a grid by index.
        /// </summary>
        /// <param name="index">The position of the record.</param>
        [When(@"I open the record at position '(\d+)' in the related grid")]
        [When(@"I open the (\d+(?:(?:st)|(?:nd)|(?:rd)|(?:th))) record in the related grid")]
        public static void WhenIOpenTheRecordAtPositionInTheRelatedGrid(int index)
        {
            XrmApp.Grid.OpenRecord(index);
        }

        /// <summary>
        /// Opens a tab under the 'Related' tab.
        /// </summary>
        /// <param name="relatedTabName">The name of the tab under the 'Related' tab.</param>
        [When(@"I open the related '(.*)' tab")]
        public static void WhenIOpenTheRelatedTab(string relatedTabName)
        {
            XrmApp.Entity.SelectTab("Related", relatedTabName);
        }

        /// <summary>
        /// Clicks on a button in a related grid.
        /// </summary>
        /// <param name="buttonName">The name of the button.</param>
        [When(@"I click the '(.*)' button on the related grid")]
        public static void WhenIClickTheButtonOnTheRelatedGrid(string buttonName)
        {
            XrmApp.Entity.RelatedGrid.ClickCommand(buttonName);
        }

        /// <summary>
        /// Asserts that a button is available in a button flyout on the related grid.
        /// </summary>
        /// <param name="button">The name of the button.</param>
        [Then(@"I should see a '(.*)' button in the flyout on the related grid")]
        public static void ThenIShouldSeeAButtonInTheFlyoutOnTheRelatedGrid(string button)
        {
            var overFlowContainer = Driver.FindElement(By.XPath(AppElements.Xpath[AppReference.Related.CommandBarOverflowContainer]));

            overFlowContainer
                .Invoking(container => container.FindElement(By.XPath(AppElements.Xpath[AppReference.Related.CommandBarSubButton].Replace("[NAME]", button))))
                .Should()
                .NotThrow(because: "the button in the flyout should exist");
        }

        /// <summary>
        /// Asserts that a button is not available in a button flyout on the related grid.
        /// </summary>
        /// <param name="button">The name of the button.</param>
        [Then(@"I should not see a '(.*)' button in the flyout on the related grid")]
        public static void ThenIShouldNotSeeAButtonInTheFlyoutOnTheRelatedGrid(string button)
        {
            var overFlowContainer = Driver.FindElement(By.XPath(AppElements.Xpath[AppReference.Related.CommandBarOverflowContainer]));

            overFlowContainer
                .Invoking(container => container.FindElement(By.XPath(AppElements.Xpath[AppReference.Related.CommandBarSubButton].Replace("[NAME]", button))))
                .Should()
                .Throw<NoSuchElementException>(because: "the button in the flyout shouldn't exist");
        }
    }
}
