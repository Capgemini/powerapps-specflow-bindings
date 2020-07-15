namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Capgemini.PowerApps.SpecFlowBindings.Extensions;
    using FluentAssertions;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
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
        /// Selects a record in a lookup.
        /// </summary>
        /// <param name="recordName">The name of the record.</param>
        /// <param name="lookupName">The logical name of the lookup.</param>
        [When(@"I select a record named '(.*)' in the '(.*)' lookup")]
        public static void WhenISelectARecordNamedInTheLookup(string recordName, string lookupName)
        {
            XrmApp.Entity.SetValue(new LookupItem { Name = lookupName, Value = recordName });
        }

        /// <summary>
        /// Performs an search in a lookup to show available records.
        /// </summary>
        /// <param name="searchString">The string to search.</param>
        /// <param name="lookupName">The lookup logical name.</param>
        [When(@"I search for '(.*)' in the '(.*)' lookup")]
        public static void WhenIClickToBrowseRecordsInTheLookup(string searchString, string lookupName)
        {
            Driver.WaitForTransaction();
            var fieldContainer = Driver.WaitUntilAvailable(By.XPath(AppElements.Xpath[AppReference.Entity.TextFieldContainer].Replace("[NAME]", lookupName)));
            var input = fieldContainer.FindElement(By.TagName("input"));
            input.Click();
            input.SendKeys(searchString);
            input.SendKeys(Keys.Enter);
        }

        /// <summary>
        /// Asserts that the given record names are visible in a lookup flyout.
        /// </summary>
        /// <param name="lookupName">The name of the lookup.</param>
        /// <param name="recordNames">The names of the records that should be visible.</param>
        [Then(@"I can see only the following records in the '(.*)' lookup")]
        public static void ThenICanSeeOnlyTheFollowingRecordsInTheLookup(string lookupName, Table recordNames)
        {
            XrmApp.ThinkTime(1000);

            if (recordNames is null)
            {
                throw new ArgumentNullException(nameof(recordNames));
            }

            IWebElement flyout = null;

            try
            {
                flyout = Driver.FindElement(By.CssSelector("[data-id*=SimpleLookupControlFlyout]"));
            }
            catch (NoSuchElementException ex)
            {
                throw new ElementNotVisibleException($"The flyout is not visible for the {lookupName} lookup.", ex);
            }

            var items = flyout.FindElements(By.CssSelector("ul[data-id*=LookupResultsDropdown] li[data-id*=LookupResultsDropdown] label:first-child")).Select(e => e.Text).ToList();

            items.Count.Should().Be(recordNames.Rows.Count, because: "the flyout should only contain the given records.");
            foreach (var item in items)
            {
                recordNames.Rows.Should().Contain(r => r[0] == item, because: "every given records should be present in the flyout.");
            }
        }

        /// <summary>
        /// Selects a tab on the form.
        /// </summary>
        /// <param name="tabName">The name of the tab.</param>
        [Given(@"I select the '(.*)' tab")]
        [When(@"I select the '(.*)' tab")]
        public static void ISelectTab(string tabName)
        {
            Driver.WaitUntilVisible(
                By.CssSelector($"li[title=\"{tabName}\"]"));

            XrmApp.Entity.SelectTab(tabName);
        }

        /// <summary>
        /// Sets the value for the field.
        /// </summary>
        /// <param name="fieldValue">The Field Value.</param>
        /// <param name="fieldName">The Field Name.</param>
        /// <param name="fieldType">The Field Type.</param>
        [When(@"I enter '(.*)' into the '(.*)' (text|optionset|boolean|numeric|currency|datetime|lookup) field")]
        public static void WhenIEnterInTheField(string fieldValue, string fieldName, string fieldType)
        {
            SetFieldValue(fieldName, fieldValue.ReplaceTemplatedText(), fieldType);
        }

        /// <summary>
        /// Select a Form.
        /// </summary>
        /// <param name="formName">The name of the form.</param>
        [When(@"I select '(.*)' form")]
        public static void WhenISelectForm(string formName)
        {
            XrmApp.Entity.SelectForm(formName);
        }

        /// <summary>
        /// Saves the record.
        /// </summary>
        [When(@"I save the record")]
        public static void WhenISaveTheRecord()
        {
            XrmApp.Entity.Save();
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
        /// Asserts that a field is editable on the form.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        [Then(@"I can edit the '(.*)' field")]
        public static void ThenICanEditTheField(string fieldName)
        {
            XrmApp.Entity.GetField(fieldName).IsVisible.Should().BeTrue(because: "the field must be visible to be editable");
            XrmApp.Entity.GetField(fieldName).IsReadOnly(Driver).Should().BeFalse(because: "the field should be editable");
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
        /// Asserts that a field is read-only.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        [Then(@"I can not edit the '(.*)' field")]
        public static void ThenICanNotEditTheField(string fieldName)
        {
            XrmApp.Entity.GetField(fieldName).IsReadOnly(Driver).Should().BeTrue(because: "the field should not be editable");
        }

        /// <summary>
        /// Asserts that the provided fields are not editable.
        /// </summary>
        /// <param name="table">A table containing the fields to assert against.</param>
        [Then(@"I can not edit the following fields")]
        public static void ThenICanNotEditTheFollowingFields(Table table)
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
                field.IsReadOnly.Should().BeTrue();
            }
        }

        /// <summary>
        /// Asserts that a field is not visible.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        [Then(@"I can see the '(.*)' field")]
        public static void ThenICanSeeTheField(string fieldName)
        {
            XrmApp.Entity.IsFieldVisible(Driver, fieldName).Should().BeTrue(because: "the field should be visible");
        }

        /// <summary>
        /// Asserts that a field is not visible.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        [Then(@"I can not see the '(.*)' field")]
        public static void ThenICanNotSeeTheField(string fieldName)
        {
            XrmApp.Entity.IsFieldVisible(Driver, fieldName).Should().BeFalse(because: "the field should not be visible");
        }

        private static void SetFieldValue(string fieldName, string fieldValue, string fieldType)
        {
            switch (fieldType)
            {
                case "optionset":
                    XrmApp.Entity.SetValue(new OptionSet()
                    {
                        Name = fieldName,
                        Value = fieldValue,
                    });
                    break;
                case "boolean":
                    XrmApp.Entity.SetValue(new BooleanItem()
                    {
                        Name = fieldName,
                        Value = bool.Parse(fieldValue),
                    });
                    break;
                case "datetime":
                    XrmApp.Entity.SetValue(new DateTimeControl(fieldName)
                    {
                        Value = DateTime.Parse(fieldValue, CultureInfo.CreateSpecificCulture("en-GB")),
                    });
                    break;
                case "lookup":
                    XrmApp.Entity.SetValue(new LookupItem()
                    {
                        Name = fieldName,
                        Value = fieldValue,
                    });
                    break;
                case "currency":
                case "numeric":
                case "text":
                default:
                    XrmApp.Entity.SetValue(fieldName, fieldValue);
                    break;
            }
        }
    }
}
