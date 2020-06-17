namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Capgemini.PowerApps.SpecFlowBindings.Extensions;
    using FluentAssertions;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using OpenQA.Selenium;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Step bindings related to forms.
    /// </summary>
    [Binding]
    public class EntitySteps : PowerAppsStepDefiner
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
        /// Asserts that the provided form for the provided entity is shown.
        /// </summary>
        /// <param name="formName">The name of the form.</param>
        /// <param name="entityName">The name of the entity.</param>
        [Then(@"I am presented with a new '(.*)' form for the '(.*)' entity")]
        public static void ThenIAmPresentedWithANewFormForTheEntity(string formName, string entityName)
        {
            XrmApp.Entity.GetFormName().Should().Be(formName);
            XrmApp.Entity.GetEntityName().Should().Be(entityName);
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
                GetSubGridItemIndexByAlias(row[0], subGridName)
                    .HasValue
                    .Should().BeTrue(because: "the record should be found in the grid");
            }
        }

        /// <summary>
        /// Asserts that the provided fields are editable.
        /// </summary>
        /// <param name="table">A table containing the fields to assert against.</param>
        [Then(@"I can edit the following fields")]
        public static void ThenICanEditTheFollowingFields(Table table)
        {
            if (table is null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            var fields = table.Rows.Select((row) => XrmApp.Entity.GetField(row.Values.First()));

            foreach (var field in fields)
            {
                field.Should().NotBeNull();
                field.IsVisible.Should().BeTrue();
                field.IsReadOnly.Should().BeFalse();
            }
        }

        /// <summary>
        /// Asserts that the subgrid contains a record that has a reference to the given test record in a given column.
        /// </summary>
        /// <param name="subGridName">The name of the subgrid.</param>
        /// <param name="alias">The alias of the test record.</param>
        /// <param name="lookup">The logical name of the lookup column.</param>
        [Then(@"the '(.*)' subgrid contains a record with '(.*)' in the '(.*)' lookup")]
        public void ThenTheSubgridContainsARecordWithInTheLookup(string subGridName, string alias, string lookup)
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
        public void ThenTheSubgridContainsRecordsWithTheFollowingInTheLookup(string subGridName, string lookup, Table table)
        {
            if (table is null)
            {
                throw new System.ArgumentNullException(nameof(table));
            }

            foreach (var row in table.Rows)
            {
                var alias = row[0];
                var reference = TestDriver.GetTestRecordReference(alias);
                var index = (long)Driver.ExecuteScript(
                    $"return Xrm.Page.getControl(\"{subGridName}\").getGrid().getRows().get().findIndex(row => row.data.entity.attributes.get().findIndex(a => a.getName() === \"{lookup}\" && a.getValue() && a.getValue()[0].id === \"{reference.Id.ToString("B").ToUpper(CultureInfo.CurrentCulture)}\") > -1)");
                index.Should().BeGreaterOrEqualTo(0, because: "a matching record should be present in the subgrid");
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
