namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Capgemini.PowerApps.SpecFlowBindings.Extensions;
    using FluentAssertions;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using OpenQA.Selenium;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Step bindings related to subgrids on forms.
    /// </summary>
    [Binding]
    public class EntitySubGridSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Clicks a command on a subgrid.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="subGridName">The name of the subgrid.</param>
        [When(@"I click the '(.*)' command on the '(.*)' subgrid")]
        public static void WhenISelectTheCommandOnTheSubgrid(string commandName, string subGridName)
        {
            Driver.WaitUntilVisible(
                By.CssSelector($"div#dataSetRoot_{subGridName} button[aria-label=\"{commandName}\"]"));
            XrmApp.Entity.SubGrid.ClickCommand(subGridName, commandName);
        }

        /// <summary>
        /// Selects a previously created test record in a subgrid.
        /// </summary>
        /// <param name="alias">The alias of the test record.</param>
        /// <param name="subGridName">The name of the subgrid.</param>
        [When(@"I select '(.*)' from the '(.*)' subgrid")]
        public static void WhenISelectFromTheSubgrid(string alias, string subGridName)
        {
            var index = GetSubGridItemIndexByAlias(alias, subGridName);
            index.HasValue.Should().BeTrue(because: "the record should be found in the grid");

            XrmApp.Entity.SubGrid.HighlightRecord(subGridName, Driver, index.Value);
        }

        /// <summary>
        /// Selects all records in a subgrid.
        /// </summary>
        /// <param name="subGridName">The name of the subgrid.</param>
        [When(@"I select all in the '(.*)' subgrid")]
        public static void WhenISelectAllInTheSubgrid(string subGridName)
        {
            XrmApp.Entity.SubGrid.ClickSubgridSelectAll(subGridName);
        }

        /// <summary>
        /// Selects a records in a subgrid by index.
        /// </summary>
        /// <param name="index">The position of the record.</param>
        /// <param name="subGridName">The name of the subgrid.</param>
        [When(@"I open the record at position '(\d+)' in the '(.*)' subgrid")]
        public static void WhenIOpenTheRecordAtPositionInTheSubgrid(int index, string subGridName)
        {
            XrmApp.Entity.SubGrid.OpenSubGridRecord(subGridName, index);
        }

        /// <summary>
        /// Searches in a subgrid.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <param name="subGridName">The name of the subgrid.</param>
        [When(@"I search for '(.*)' in the '(.*)' subgrid")]
        public static void WhenISearchForInTheSubgrid(string text, string subGridName)
        {
            XrmApp.Entity.SubGrid.Search(subGridName, text);
        }

        /// <summary>
        /// Switches view in a subgrid.
        /// </summary>
        /// <param name="viewName">The name of the view.</param>
        /// <param name="subGridName">The name of the subgrid.</param>
        [When(@"I switch to the '(.*)' view in the '(.*)' subgrid")]
        public static void WhenISwitchToTheViewInTheSubgrid(string viewName, string subGridName)
        {
            XrmApp.Entity.SubGrid.SwitchView(subGridName, viewName);
        }

        /// <summary>
        /// Asserts that a record is not in a subgrid.
        /// </summary>
        /// <param name="alias">The alias of the test record.</param>
        /// <param name="subGridName">The name of the subgrid.</param>
        [Then(@"I can not see '(.*)' in the '(.*)' subgrid")]
        public static void ThenICanNotSeeInTheSubgrid(string alias, string subGridName)
        {
            GetSubGridItemIndexByAlias(alias, subGridName)
                .HasValue
                .Should().BeFalse(because: "the record should not exist in the subgrid");
        }

        /// <summary>
        /// Asserts that a given number of records are present in the subgrid.
        /// </summary>
        /// <param name="compare">The type of comparison.</param>
        /// <param name="count">The count to compare.</param>
        /// <param name="subGridName">The name of the subgrid.</param>
        [Then(@"I can see (exactly|more than|less than) (\d+) records in the '(.*)' subgrid")]
        public static void ThenICanSeeRecordsInTheSubgrid(string compare, int count, string subGridName)
        {
            var actualCount = XrmApp.Entity.SubGrid.GetSubGridItemsCount(subGridName);

            switch (compare)
            {
                case "exactly":
                    actualCount.Should().Be(count);
                    break;
                case "more than":
                    actualCount.Should().BeGreaterThan(count);
                    break;
                case "less than":
                    actualCount.Should().BeLessThan(count);
                    break;
            }
        }

        /// <summary>
        /// Asserts that the specified subgrid contains the specified test record.
        /// </summary>
        /// <param name="subGridName">The name of the subgrid.</param>
        /// <param name="recordAlias">The alias of the test record.</param>
        [Then(@"the '(.*)' subgrid contains '(.*)'")]
        public static void ThenTheGridContains(string subGridName, string recordAlias)
        {
            var index = GetSubGridItemIndexByAlias(recordAlias, subGridName);

            index.HasValue.Should().BeTrue(because: "the record should be found in the grid");
        }

        /// <summary>
        /// Asserts that the specified subgrid contains the specified test records.
        /// </summary>
        /// <param name="subGridName">The name of the subgrid.</param>
        /// <param name="table">The record aliases.</param>
        [Then(@"the '(.*)' subgrid contains the following records")]
        public static void ThenTheSubgridContainsTheFollowingRecords(string subGridName, Table table)
        {
            if (table is null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            foreach (var row in table.Rows)
            {
                ThenTheGridContains(subGridName, row[0]);
            }
        }

        /// <summary>
        /// Asserts that the subgrid contains a record that has a reference to the given test record in a given column.
        /// </summary>
        /// <param name="subGridName">The name of the subgrid.</param>
        /// <param name="alias">The alias of the test record.</param>
        /// <param name="lookup">The logical name of the lookup column.</param>
        [Then(@"the '(.*)' subgrid contains a record with a reference to '(.*)' in the '(.*)' lookup field")]
        public static void ThenTheSubgridContainsARecordWithInTheLookup(string subGridName, string alias, string lookup)
        {
            var reference = TestDriver.GetTestRecordReference(alias);

            var index = (long)Driver.ExecuteScript(
                $"return Xrm.Page.getControl(\"{subGridName}\").getGrid().getRows().get().findIndex(row => row.data.entity.attributes.get().findIndex(a => a.getName() === \"{lookup}\" && a.getValue() && a.getValue()[0].id === \"{reference.Id.ToString("B").ToUpper(CultureInfo.CurrentCulture)}\") > -1)");

            index.Should().BeGreaterOrEqualTo(0, because: "a matching record should be present in the subgrid");
        }

        /// <summary>
        /// Asserts that the subgrid contains records that have references to the given test records in a given column.
        /// </summary>
        /// <param name="subGridName">The name of the subgrid.</param>
        /// <param name="lookup">The logical name of the lookup column.</param>
        /// <param name="table">The record aliases.</param>
        [Then(@"the '(.*)' subgrid contains records with the following in the '(.*)' lookup")]
        public static void ThenTheSubgridContainsRecordsWithTheFollowingInTheLookup(string subGridName, string lookup, Table table)
        {
            if (table is null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            foreach (var row in table.Rows)
            {
                ThenTheSubgridContainsARecordWithInTheLookup(subGridName, row[0], lookup);
            }
        }

        /// <summary>
        /// Asserts that the subgrid contains a record with a field matching the criteria.
        /// </summary>
        /// <param name="subGridName">The name of the subgrid.</param>
        /// <param name="fieldValue">The expected value of the field.</param>
        /// <param name="fieldName">The name of the field.</param>
        [Then(@"the '(.*)' subgrid contains a record with '(.*)' in the '(.*)' (?:text|numeric|currency) field")]
        public static void ThenTheSubgridContainsRecordsWithInTheField(string subGridName, string fieldValue, string fieldName)
        {
            // Bug in GetSubGridItems returns the same attribute values for every record. Using deprecated Xrm.Page for now
            // List<GridItem> subGridItems = XrmApp.Entity.SubGrid.GetSubGridItems(subGridName);
            // subGridItems.Any(item => item.GetAttribute<string>(fieldName) == fieldValue)
            //   .Should().BeTrue(because: "a matching record should be present in the subgrid);
            var index = (long)Driver.ExecuteScript(
                $"return Xrm.Page.getControl(\"{subGridName}\").getGrid().getRows().get().findIndex(row => row.data.entity.attributes.get().findIndex(a => a.getName() === \"{fieldName}\" && a.getValue() == \"{fieldValue}\") > -1)");

            index.Should().BeGreaterOrEqualTo(0, because: "a matching record should be present in the subgrid");
        }

        /// <summary>
        /// Asserts that the subgrid contains a record with a lookup matching the criteria.
        /// </summary>
        /// <param name="subGridName">The name of the subgrid.</param>
        /// <param name="fieldValue">The expected value of the field.</param>
        /// <param name="fieldName">The name of the field.</param>
        [Then(@"the '(.*)' subgrid contains a record with '(.*)' in the '(.*)' lookup field")]
        public static void ThenTheSubgridContainsARecordWithInTheLookupField(string subGridName, string fieldValue, string fieldName)
        {
            // Bug in GetSubGridItems returns the same attribute values for every record. Using deprecated Xrm.Page for now
            var index = (long)Driver.ExecuteScript(
                $"return Xrm.Page.getControl(\"{subGridName}\").getGrid().getRows().get().findIndex(row => row.data.entity.attributes.get().findIndex(a => a.getName() === \"{fieldName}\" && a.getValue() && a.getValue()[0].name === \"{fieldValue}\") > -1)");

            index.Should().BeGreaterOrEqualTo(0, because: "a matching record should be present in the subgrid");
        }

        /// <summary>
        /// Asserts the command is visible on the subgrid.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="subGridName">The name of the subgrid.</param>
        [Then(@"I can see the '(.*)' command on the '(.*)' subgrid")]
        public static void ThenICanSeeTheCommandOnTheSubgrid(string commandName, string subGridName)
        {
            Driver.WaitUntilVisible(
                By.CssSelector($"div#dataSetRoot_{subGridName} button[aria-label=\"{commandName}\"]"),
                new TimeSpan(0, 0, 5),
                $"Could not find the {commandName} command on the {subGridName} subgrid.");
        }

        /// <summary>
        /// Clicks a flyout on a subgrid.
        /// </summary>
        /// <param name="flyoutName">The name of the flyout.</param>
        /// <param name="subGridName">The name of the subgrid.</param>
        [When(@"I click the '([^']+)' flyout on the '([^']+)' subgrid")]
        public static void WhenIClickTheFlyoutOnTheSubgrid(string flyoutName, string subGridName)
        {
            Driver.WaitUntilVisible(By.CssSelector($"div#dataSetRoot_{subGridName} li[aria-label=\"{flyoutName}\"]"));

            XrmApp.Entity.SubGrid.ClickCommand(subGridName, flyoutName);
        }

        /// <summary>
        /// Asserts the command is not visible on the subgrid.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="subGridName">The name of the subgrid.</param>
        [Then(@"I can not see the '(.*)' command on the '(.*)' subgrid")]
        public static void ThenICanNotSeeTheCommandOnTheSubgrid(string commandName, string subGridName)
        {
            Driver
                .Invoking(d => d.WaitUntilVisible(
                    By.CssSelector($"div#dataSetRoot_{subGridName} button[aria-label=\"{commandName}\"]"),
                    new TimeSpan(0, 0, 5)))
                .Should()
                .Throw<Exception>();
        }

        /// <summary>
        /// Asserts the command is visible on the open flyout of the subgrid.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        [Then(@"I can see the '(.*)' command on the flyout of the subgrid")]
        public static void ThenICanSeeTheCommandOnTheFlyoutOfTheSubgrid(string commandName)
        {
            Driver.WaitUntilVisible(
                By.CssSelector($"div[data-id*=\"flyoutRootNode\"] button[aria-label='{commandName}']"),
                new TimeSpan(0, 0, 1),
                $"Could not find the {commandName} command on the flyout of the subgrid.");
        }

        /// <summary>
        /// Asserts the command is not visible on the open flyout of the subgrid.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        [Then(@"I can not see the '(.*)' command on the flyout of the subgrid")]
        public static void ThenICanNotSeeTheCommandOnTheFlyoutOfTheSubgrid(string commandName)
        {
            Driver
                .Invoking(d => d.WaitUntilVisible(
                    By.CssSelector($"div[data-id*=\"flyoutRootNode\"] button[aria-label=\"{commandName}\"]"),
                    new TimeSpan(0, 0, 1),
                    $"Could not find the {commandName} command on the flyout of the subgrid."))
                .Should()
                .Throw<Exception>();
        }

        /// <summary>
        /// Clicks a command under a flyout on a subgrid.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="flyoutName">The name of the flyout.</param>
        /// <param name="subGridName">The name of the subgrid.</param>
        [When(@"I click the '([^']+)' command under the '([^']+)' flyout on the '([^']+)' subgrid")]
        public static void WhenIClickTheCommandUnderTheFlyoutOnTheSubgrid(string commandName, string flyoutName, string subGridName)
        {
            /* Temporary until https://github.com/microsoft/EasyRepro/pull/918 is approved, then it can just be:
                    XrmApp.Entity.SubGrid.ClickCommand(subGridName, flyoutName, commandName);
             */

            WhenIClickTheFlyoutOnTheSubgrid(flyoutName, subGridName);

            Driver
                .WaitUntilVisible(
                    By.CssSelector($"div[data-id*=\"flyoutRootNode\"] button[aria-label=\"{commandName}\"]"),
                    new TimeSpan(0, 0, 1),
                    $"Could not find the {commandName} command on the flyout of the subgrid.")
                .Click(false);
        }

        /// <summary>
        /// Selects a record in the subgrid given a value is in a particular field.
        /// </summary>
        /// <param name="fieldValue">The field value.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="subGridName">The subgrid name.</param>
        [When(@"I select a record with '(.*)' in the '(.*)' field in the '(.*)' subgrid")]
        public static void WhenISelectARecordWithInTheFieldInTheSubgrid(string fieldValue, string fieldName, string subGridName)
        {
            List<GridItem> subGridItems = XrmApp.Entity.SubGrid.GetSubGridItems(subGridName);

            GridItem subGridItem = subGridItems.FirstOrDefault(item => item.GetAttribute<string>(fieldName) == fieldValue);

            if (subGridItem != default(GridItem))
            {
                XrmApp.Entity.SubGrid.HighlightRecord(subGridName, Driver, subGridItems.IndexOf(subGridItem));
            }
            else
            {
                throw new Exception($"Could not find record with field '{fieldName}' with value '{fieldValue}' in the '{subGridName}' subgrid.");
            }
        }

        private static int? GetSubGridItemIndexByAlias(string recordAlias, string subGridName)
        {
            var record = TestDriver.GetTestRecordReference(recordAlias);
            var index = XrmApp.Entity.SubGrid.GetRecordIndexById(subGridName, Driver, record.Id);

            return index > -1 ? index : default(int?);
        }
    }
}
