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
        /// Selects a previously created test record from a grid.
        /// </summary>
        /// <param name="recordAlias">The alias of the test record.</param>
        [When(@"I select '(.*)' from the grid")]
        public static void WhenISelectFromTheGrid(string recordAlias)
        {
            HighlightRowByAlias(recordAlias);
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
        /// <param name="table">The table containing the record aliases</param>
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
