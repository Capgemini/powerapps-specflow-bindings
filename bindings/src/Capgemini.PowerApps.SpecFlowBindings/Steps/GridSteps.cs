namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using Capgemini.PowerApps.SpecFlowBindings.Extensions;
    using FluentAssertions;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Step bindings related to grids.
    /// </summary>
    [Binding]
    public class GridSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Selects a records in a grid by index.
        /// </summary>
        /// <param name="index">The position of the record.</param>
        [When(@"I open the record at position '(\d+)' in the grid")]
        [When(@"I open the (\d+(?:(?:st)|(?:nd)|(?:rd)|(?:th))) record in the grid")]
        public static void WhenIOpenTheRecordAtPositionInTheGrid(int index)
        {
            XrmApp.Grid.OpenRecord(index);
        }

        /// <summary>
        /// Selects a previously created test record from a grid.
        /// </summary>
        /// <param name="recordAlias">The alias of the test record.</param>
        [When(@"I select '(.*)' from the grid")]
        public static void WhenIOpenTheRecordAtPositionInTheGrid(string recordAlias)
        {
            HighlightRowByAlias(recordAlias);
        }

        /// <summary>
        /// Clears the search in a grid.
        /// </summary>
        [When(@"I clear the search in the grid")]
        public static void WhenIClearTheSearchInTheGrid()
        {
            XrmApp.Grid.ClearSearch();
        }

        /// <summary>
        /// Sorts by a column in the grid.
        /// </summary>
        /// <param name="column">The column to sort by.</param>
        /// <param name="sortOption">The sort option.</param>
        [When(@"I sort the '(.*)' column in the grid using the '(.*)' option")]
        public static void WhenISortByInTheGrid(string column, string sortOption)
        {
            XrmApp.Grid.Sort(column, sortOption);
        }

        /// <summary>
        /// Switches view in a grid.
        /// </summary>
        /// <param name="viewName">The name of the view.</param>
        [When(@"I switch to the '(.*)' view in the grid")]
        public static void WhenISwitchToTheViewInTheGrid(string viewName)
        {
            XrmApp.Grid.SwitchView(viewName);
        }

        /// <summary>
        /// Navigates to an entity list for the given entity.
        /// </summary>
        /// <param name="entityName">The name of the entity to show the list for.</param>
        [Given(@"I am viewing a list of '(.*)' records")]
        public static void GivenIAmViewingAList(string entityName)
        {
            var urlPart = Driver.Url.Substring(0, Driver.Url.IndexOf("&pagetype="));
            var url = urlPart + "&pagetype=entitylist&etn=" + entityName;
            Driver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Asserts that the specified grid contains the specified test record.
        /// </summary>
        /// <param name="recordAlias">The alias of the test record.</param>
        [Then(@"the grid contains '(.*)'")]
        public static void ThenTheGridContains(string recordAlias)
        {
            var index = GetGridItemIndexByAlias(recordAlias);

            index.HasValue.Should().BeTrue(because: "the record should be found in the grid.");
        }

        /// <summary>
        /// Selects multiple previously created test records from a grid.
        /// </summary>
        /// <param name="table">The table containing the record aliases.</param>
        [When(@"I select the following records from the grid")]
        public static void WhenISelectTheFollowingRecordsFromTheGrid(Table table)
        {
            if (table is null)
            {
                throw new System.ArgumentNullException(nameof(table));
            }

            foreach (var row in table.Rows)
            {
                HighlightRowByAlias(row[0]);
            }
        }

        /// <summary>
        /// Searches for a string on the grid.
        /// </summary>
        /// <param name="searchString">The string to search for.</param>
        [When(@"I search for '(.*)' in the grid")]
        public static void WhenISearchForInTheGrid(string searchString)
        {
            XrmApp.Grid.Search(searchString);

            Driver.WaitForTransaction();
        }

        private static void HighlightRowByAlias(string alias)
        {
            var index = GetGridItemIndexByAlias(alias);
            index.HasValue.Should().BeTrue(because: "the record should be found in the grid.");

            XrmApp.Grid.HighLightRecord(index.Value);
        }

        private static int? GetGridItemIndexByAlias(string recordAlias)
        {
            var record = TestDriver.GetTestRecordReference(recordAlias);

            /*
             * TODO: Replace extension method with the below code when the following bug is resolved: https://github.com/microsoft/EasyRepro/issues/800.
             * var index = XrmApp.Grid.GetGridItems().FindIndex(i => i.Id == record.Id);
             */
            var index = XrmApp.Grid.GetRecordIndexById(Driver, record.Id);

            return index > -1 ? index : default(int?);
        }
    }
}
