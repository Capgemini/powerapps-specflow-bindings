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
        /// Selects a tab on the form.
        /// </summary>
        /// <param name="tabName">The name of the tab.</param>
        [When(@"I select the '(.*)' tab")]
        public static void ISelectTab(string tabName)
        {
            Driver.WaitUntilVisible(By.CssSelector($"li[title=\"{tabName}\"]"));

            XrmApp.Entity.SelectTab(tabName);
        }

        /// <summary>
        /// Asserts whether a tab is currently visible.
        /// </summary>
        /// <param name="canOrCannot">Whether the tab should be visible.</param>
        /// <param name="tabName">The name of the tab.</param>
        [Then(@"I (can|cannot) see the '(.*)' tab")]
        public static void ICanSeeTab(string canOrCannot, string tabName)
        {
            canOrCannot = canOrCannot ?? throw new ArgumentNullException(nameof(canOrCannot));
            tabName = tabName ?? throw new ArgumentNullException(nameof(tabName));

            bool shouldBeVisible = canOrCannot.Equals("can", StringComparison.InvariantCultureIgnoreCase);

            Driver.IsVisible(By.CssSelector($"li[title=\"{tabName}\"]"))
                .Should()
                .Be(
                    shouldBeVisible,
                    because: $"The tab '{tabName}' {(shouldBeVisible ? "should" : "should not")} be visible.");
        }

        /// <summary>
        /// Sets the value for the field.
        /// </summary>
        /// <param name="fieldValue">The field value.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldType">The field type.</param>
        /// <param name="fieldLocation">Whether the field is in the header.</param>
        [When(@"I enter '(.*)' into the '(.*)' (text|optionset|multioptionset|boolean|numeric|currency|datetime|lookup) (field|header field) on the form")]
        public static void WhenIEnterInTheField(string fieldValue, string fieldName, string fieldType, string fieldLocation)
        {
            if (fieldLocation == "field")
            {
                SetFieldValue(fieldName, fieldValue.ReplaceTemplatedText(), fieldType);
            }
            else
            {
                SetHeaderFieldValue(fieldName, fieldValue.ReplaceTemplatedText(), fieldType);
            }

            // Click to lose focus - So that business rules and other form events can occur
            Driver.FindElement(By.XPath("html")).Click();

            Driver.WaitForTransaction();
        }

        /// <summary>
        /// Sets the values of the fields in the table on the form.
        /// </summary>
        /// <param name="fields">The fields to set.</param>
        [When(@"I enter the following into the form")]
        public static void WhenIEnterTheFollowingIntoTheForm(Table fields)
        {
            fields = fields ?? throw new ArgumentNullException(nameof(fields));

            foreach (TableRow row in fields.Rows)
            {
                WhenIEnterInTheField(row["Value"], row["Field"], row["Type"], row["Location"]);
            }
        }

        /// <summary>
        /// Clears the value for the field.
        /// </summary>
        /// <param name="field">The field name.</param>
        [When(@"I clear the '(.*)' (?:currency|numeric|text|boolean) field")]
        public static void WhenIClearTheField(string field)
        {
            XrmApp.Entity.ClearValue(field);
        }

        /// <summary>
        /// Clears the value for the optionset field.
        /// </summary>
        /// <param name="field">The field.</param>
        [When(@"I clear the '(.*)' optionset field")]
        public static void WhenIClearTheOptionSetField(OptionSet field)
        {
            XrmApp.Entity.ClearValue(field);
        }

        /// <summary>
        /// Clears the value for the boolean field.
        /// </summary>
        /// <param name="field">The field.</param>
        [When(@"I clear the '(.*)' datetime field")]
        public static void WhenIClearTheDateTimeField(DateTimeControl field)
        {
            XrmApp.Entity.ClearValue(field);
        }

        /// <summary>
        /// Clears the value for the lookup field.
        /// </summary>
        /// <param name="field">The field.</param>
        [When(@"I clear the '(.*)' lookup field")]
        public static void WhenIClearTheLookupField(LookupItem field)
        {
            XrmApp.Entity.ClearValue(field);
        }

        /// <summary>
        /// Deletes the record.
        /// </summary>
        [When(@"I delete the record")]
        public static void WhenIDeleteTheRecord()
        {
            XrmApp.Entity.Delete();
        }

        /// <summary>
        /// Opens the record set navigator.
        /// </summary>
        [When(@"I open the record set navigator")]
        public static void WhenIOpenTheRecordSetNavigator()
        {
            XrmApp.Entity.OpenRecordSetNavigator();
        }

        /// <summary>
        /// Closes the record set navigator.
        /// </summary>
        [When(@"I close the record set navigator")]
        public static void WhenICloseTheRecordSetNavigator()
        {
            XrmApp.Entity.CloseRecordSetNavigator();
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
        /// Select a lookup on the form.
        /// </summary>
        /// <param name="fieldName">The name of the lookup.</param>
        [When(@"I select '(.*)' lookup")]
        public static void WhenISelectLookup(string fieldName)
        {
            XrmApp.Entity.SelectLookup(new LookupItem { Name = fieldName });
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
        /// Assigns the record to a user or team.
        /// </summary>
        /// <param name="userOrTeamName">The name of the user or team.</param>
        [When(@"I assign the record to a (?:user|team) named '(.*)'")]
        public static void WhenIAssignTheRecordToANamed(string userOrTeamName)
        {
            XrmApp.Entity.Assign(userOrTeamName);
        }

        /// <summary>
        /// Switches business process flow.
        /// </summary>
        /// <param name="process">The name of the process.</param>
        [When(@"I switch process to the '(.*)' process")]
        public static void WhenISwitchProcessToTheProcess(string process)
        {
            XrmApp.Entity.SwitchProcess(process);
        }

        /// <summary>
        /// Asserts that a value is shown in a text, currency or numeric field.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="field">The field name.</param>
        /// <param name="fieldLocation">Where the field is located.</param>
        [Then("I can see a value of '(.*)' in the '(.*)' (?:currency|numeric|text) (field|header field)")]
        public static void ThenICanSeeAValueOfInTheField(string expectedValue, string field, string fieldLocation)
        {
            string actualValue = fieldLocation == "field" ? XrmApp.Entity.GetValue(field) : XrmApp.Entity.GetHeaderValue(field);
            actualValue.Should().Be(expectedValue);
        }

        /// <summary>
        /// Asserts that a value is shown in an option set field.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="field">The field name.</param>
        /// <param name="fieldLocation">Where the field is located.</param>
        [Then("I can see a value of '(.*)' in the '(.*)' optionset (field|header field)")]
        public static void ThenICanSeeAValueOfInTheOptionSetField(string expectedValue, OptionSet field, string fieldLocation)
        {
            string actualValue = fieldLocation == "field" ? XrmApp.Entity.GetValue(field) : XrmApp.Entity.GetHeaderValue(field);
            actualValue.Should().Be(expectedValue);
        }

        /// <summary>
        /// Asserts that a value is shown in a lookup field.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="field">The field name.</param>
        /// <param name="fieldLocation">Where the field is located.</param>
        [Then("I can see a value of '(.*)' in the '(.*)' lookup (field|header field)")]
        public static void ThenICanSeeAValueOfInTheOptionSetField(string expectedValue, LookupItem field, string fieldLocation)
        {
            string actualValue = fieldLocation == "field" ? XrmApp.Entity.GetValue(field) : XrmApp.Entity.GetHeaderValue(field);
            actualValue.Should().Be(expectedValue);
        }

        /// <summary>
        /// Asserts that a field is mandatory or optional.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="requirementLevel">Whether the field should be mandatory or optional.</param>
        [Then(@"the '(.*)' field is (mandatory|optional)")]
        public static void ThenTheFieldIsMandatory(string fieldName, string requirementLevel)
        {
            if (!Driver.TryFindElement(By.CssSelector($"div[data-id=\"{fieldName}-FieldSectionItemContainer\"]"), out var fieldContainer))
            {
                throw new InvalidOperationException($"The {fieldName} field is not visible on the form.");
            }

            fieldContainer.GetAttribute<int>("data-fieldrequirement").Should().Be(requirementLevel == "mandatory" ? 2 : 0, because: $"the field should be {requirementLevel}");
        }

        /// <summary>
        /// Asserts that a value is shown in a boolean field.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="field">The field name.</param>
        /// <param name="fieldLocation">Where the field is located.</param>
        [Then("I can see a value of '(true|false)' in the '(.*)' boolean (field|header field)")]
        public static void ThenICanSeeAValueOfInTheBooleanField(bool expectedValue, BooleanItem field, string fieldLocation)
        {
            var actualValue = fieldLocation == "field" ? XrmApp.Entity.GetValue(field) : XrmApp.Entity.GetHeaderValue(field);
            actualValue.Should().Be(expectedValue);
        }

        /// <summary>
        /// Asserts that a value is shown in a datetime field.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="field">The field name.</param>
        /// <param name="fieldLocation">Where the field is located.</param>
        [Then(@"I can see a value of '((?:0?[1-9]|[12][0-9]|3[01])[\/\-](?:0?[1-9]|1[012])[\/\-]\d{4}(?: \d{1,2}[:-]\d{2}(?:[:-]\d{2,3})*)?)' in the '(.*)' datetime (field|header field)")]
        public static void ThenICanSeeAValueOfInTheDateTimeField(DateTime expectedValue, DateTimeControl field, string fieldLocation)
        {
            var actualValue = fieldLocation == "field" ? XrmApp.Entity.GetValue(field) : XrmApp.Entity.GetHeaderValue(field);
            actualValue.Should().Be(expectedValue);
        }

        /// <summary>
        /// Asserts that a business process error is shown with a given message.
        /// </summary>
        /// <param name="expectedMessage">The expected message.</param>
        [Then(@"I can see a business process error stating '(.*)'")]
        public static void ThenICanSeeABusinessProcessErrorStating(string expectedMessage)
        {
            XrmApp.Entity.GetBusinessProcessError().Should().Be(expectedMessage);
        }

        /// <summary>
        /// Asserts that the provided form for the provided entity is shown.
        /// </summary>
        /// <param name="formName">The name of the form.</param>
        /// <param name="entityName">The name of the entity.</param>
        [Then(@"I am presented with a '(.*)' form for the '(.*)' entity")]
        public static void ThenIAmPresentedWithANewFormForTheEntity(string formName, string entityName)
        {
            XrmApp.Entity.GetFormName().Should().Be(formName);
            XrmApp.Entity.GetEntityName().Should().Be(entityName);
        }

        /// <summary>
        /// Asserts that a form notification can be seen with the given message.
        /// </summary>
        /// <param name="message">The message of the notification.</param>
        [Then(@"I can see a form notification stating '(.*)'")]
        public static void ThenICanSeeAFormNotificationStating(string message)
        {
            XrmApp.Entity.GetFormNotifications().Should().Contain(message);
        }

        /// <summary>
        /// Asserts that the given value is in the header title.
        /// </summary>
        /// <param name="message">The header title.</param>
        [Then(@"I can see a value of '(.*)' as the header title")]
        public static void ThenICanSeeAsTheHeaderTitle(string message)
        {
            XrmApp.Entity.GetHeaderTitle().Should().Contain(message);
        }

        /// <summary>
        /// Asserts that a field is editable on the form.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        [Then(@"I can edit the '(.*)' field")]
        public static void ThenICanEditTheField(string fieldName)
        {
            var field = XrmApp.Entity.GetField(fieldName);

            field.IsVisible.Should().BeTrue(because: "the field must be visible to be editable");
            field.IsReadOnly(Driver).Should().BeFalse(because: "the field should be editable");
        }

        /// <summary>
        /// Asserts that the given values are available in an option set.
        /// </summary>
        /// <param name="fieldName">The name of the option set field.</param>
        /// <param name="expectedOptionsTable">The options.</param>
        [Then(@"I can see the following options in the '(.*)' option set field")]
        public static void ThenICanSeeTheFollowingOptionsInTheOptionSetField(string fieldName, Table expectedOptionsTable)
        {
            if (expectedOptionsTable is null)
            {
                throw new ArgumentNullException(nameof(expectedOptionsTable));
            }

            if (!Driver.TryFindElement(By.CssSelector($"select[data-id*=\"{fieldName}.fieldControl-option-set-select\"]"), out var optionSet))
            {
                throw new InvalidOperationException($"Unable to find option set field {fieldName}.");
            }

            var expectedOptions = expectedOptionsTable.Rows.Select(r => r[0]);
            foreach (var option in optionSet.FindElements(By.CssSelector("option")).Where(e => e.GetAttribute("value") != "-1"))
            {
                expectedOptions.Should().Contain(option.Text, because: "the options be in the list of expected options");
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

            var fields = table.Rows.Select((row) => row.Values.First());

            foreach (var field in fields)
            {
                ThenICanEditTheField(field);
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

            foreach (var field in table.Rows.Select((row) => row.Values.First()))
            {
                ThenICanNotEditTheField(field);
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

        /// <summary>
        /// Asserts that a record is active or inactive.
        /// </summary>
        /// <param name="status">The status.</param>
        [Then("the status of the record is (active|inactive)")]
        public static void ThenTheStatusOfTheRecordIs(string status)
        {
            XrmApp.Entity.GetFooterStatusValue().Should().BeEquivalentTo(status);
        }

        private static void SetFieldValue(string fieldName, string fieldValue, string fieldType)
        {
            switch (fieldType)
            {
                case "multioptionset":
                    XrmApp.Entity.SetMultiSelectOptionSetValue(
                        Driver,
                        new MultiValueOptionSet()
                        {
                            Name = fieldName,
                            Values = fieldValue
                                        .Split(',')
                                        .Select(v => v.Trim())
                                        .ToArray(),
                        },
                        true);
                    break;
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
                        Value = DateTime.Parse(fieldValue, CultureInfo.CurrentCulture),
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

        private static void SetHeaderFieldValue(string fieldName, string fieldValue, string fieldType)
        {
            switch (fieldType)
            {
                case "multioptionset":
                    XrmApp.Entity.SetMultiSelectOptionSetValue(
                        Driver,
                        new MultiValueOptionSet()
                        {
                            Name = fieldName,
                            Values = fieldValue
                                        .Split(',')
                                        .Select(v => v.Trim())
                                        .ToArray(),
                        },
                        true);
                    break;
                case "optionset":
                    XrmApp.Entity.SetHeaderValue(new OptionSet()
                    {
                        Name = fieldName,
                        Value = fieldValue,
                    });
                    break;
                case "boolean":
                    XrmApp.Entity.SetHeaderValue(new BooleanItem()
                    {
                        Name = fieldName,
                        Value = bool.Parse(fieldValue),
                    });
                    break;
                case "datetime":
                    XrmApp.Entity.SetHeaderValue(new DateTimeControl(fieldName)
                    {
                        Value = DateTime.Parse(fieldValue, CultureInfo.CurrentCulture),
                    });
                    break;
                case "lookup":
                    XrmApp.Entity.SetHeaderValue(new LookupItem()
                    {
                        Name = fieldName,
                        Value = fieldValue,
                    });
                    break;
                case "currency":
                case "numeric":
                case "text":
                default:
                    XrmApp.Entity.SetHeaderValue(fieldName, fieldValue);
                    break;
            }
        }
    }
}
