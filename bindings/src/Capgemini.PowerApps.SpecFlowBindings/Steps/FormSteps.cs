namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Step bindings related to forms.
    /// </summary>
    [Binding]
    public class FormSteps : PowerAppsStepDefiner
    {
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
    }
}
