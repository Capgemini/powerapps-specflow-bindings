namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using System;
    using System.Globalization;
    using FluentAssertions;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using OpenQA.Selenium;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Steps relating to business process flows.
    /// </summary>
    [Binding]
    public class BusinessProcessFlowSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Closes a stage on the business process flow.
        /// </summary>
        /// <param name="stageName">The name of the stage.</param>
        [When("I close the '(.*)' stage of the business process flow")]
        public static void WhenICloseTheStageOfTheBusinessProcessFlow(string stageName)
        {
            XrmApp.BusinessProcessFlow.Close(stageName);
        }

        /// <summary>
        /// Clicks the next stage button on a stage on the business process flow.
        /// </summary>
        /// <param name="stageName">The name of the stage.</param>
        [When("I click next stage on the the '(.*)' stage of the business process flow")]
        public static void WhenIClickNextStageOnTheStageOfTheBusinessProcessFlow(string stageName)
        {
            XrmApp.BusinessProcessFlow.NextStage(stageName);
        }

        /// <summary>
        /// Pins a stage on the business process flow.
        /// </summary>
        /// <param name="stageName">The name of the stage.</param>
        [When("I pin the '(.*)' stage of the business process flow")]
        public static void WhenIPinTheStageOfTheBusinessProcessFlow(string stageName)
        {
            XrmApp.BusinessProcessFlow.Pin(stageName);
        }

        /// <summary>
        /// Selects a stage on the business process flow.
        /// </summary>
        /// <param name="stageName">The name of the stage.</param>
        [When("I select the '(.*)' stage of the business process flow")]
        public static void WhenISelectTheStageOfTheBusinessProcessFlow(string stageName)
        {
            XrmApp.BusinessProcessFlow.SelectStage(stageName);
        }

        /// <summary>
        /// Sets a stage as active on the business process flow.
        /// </summary>
        /// <param name="stageName">The name of the stage.</param>
        [When("I set the '(.*|current)' stage of the business process flow as active")]
        public static void WhenISetTheStageOfTheBusinessProcessFlowAsActive(string stageName)
        {
            XrmApp.BusinessProcessFlow.SetActive(stageName == "current" ? string.Empty : stageName);
        }

        /// <summary>
        /// Sets the value for a field on a business process flow.
        /// </summary>
        /// <param name="fieldValue">The field value.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="fieldType">The field type.</param>
        [When("I enter '(.*)' into the '(.*)' (text|optionset|boolean|numeric|currency|datetime|lookup) field on the business process flow")]
        public static void IEnterIntoTheFieldOnTheBusinessProcessFlow(string fieldValue, string fieldName, string fieldType)
        {
            SetFieldValue(fieldName, fieldValue, fieldType);

            // Click to lose focus - So that business rules and other form events can occur
            Driver.FindElement(By.XPath("html")).Click();

            Driver.WaitForTransaction();
        }

        /// <summary>
        /// Asserts that a value is shown in a text, currency or numeric field on a business process flow.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="field">The field name.</param>
        [Then("I can see a value of '(.*)' in the '(.*)' (?:currency|numeric|text|boolean|datetime) field on the business process flow")]
        public static void ThenICanSeeAValueOfInTheFieldOnTheBusinessProcessFlow(string expectedValue, string field)
        {
            XrmApp.Entity.GetValue(field).Should().Be(expectedValue);
        }

        /// <summary>
        /// Asserts that a value is shown in an option set field on a business process flow.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="field">The field name.</param>
        [Then("I can see a value of '(.*)' in the '(.*)' optionset field on the business process flow")]
        public static void ThenICanSeeAValueOfInTheOptionSetFieldOnTheBusinessProcessFlow(string expectedValue, OptionSet field)
        {
            XrmApp.Entity.GetValue(field).Should().Be(expectedValue);
        }

        /// <summary>
        /// Asserts that a value is shown in a lookup field on a business process flow.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="field">The field name.</param>
        [Then("I can see a value of '(.*)' in the '(.*)' lookup field on the business process flow")]
        public static void ThenICanSeeAValueOfInTheOptionSetFieldOnTheBusinessProcessFlow(string expectedValue, LookupItem field)
        {
            XrmApp.Entity.GetValue(field).Should().Be(expectedValue);
        }

        /// <summary>
        /// Asserts that a value is shown in a boolean field on a business process flow.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="field">The field name.</param>
        [Then("I can see a value of '(true|false)' in the '(.*)' boolean field on the business process flow")]
        public static void ThenICanSeeAValueOfInTheBooleanFieldOnTheBusinessProcessFlow(bool expectedValue, string field)
        {
            bool.Parse(XrmApp.BusinessProcessFlow.GetValue(field)).Should().Be(expectedValue);
        }

        /// <summary>
        /// Asserts that a value is shown in a datetime field on a business process flow.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="field">The field name.</param>
        [Then(@"I can see a value of '((?:0?[1-9]|[12][0-9]|3[01])[\/\-](?:0?[1-9]|1[012])[\/\-]\d{4}(?: \d{1,2}[:-]\d{2}(?:[:-]\d{2,3})*)?)' in the '(.*)' datetime field on the business process flow")]
        public static void ThenICanSeeAValueOfInTheDateTimeFieldOnTheBusinessProcessFlow(DateTime expectedValue, string field)
        {
            DateTime.Parse(XrmApp.BusinessProcessFlow.GetValue(field), CultureInfo.CurrentCulture)
                .Should().Be(expectedValue);
        }

        private static void SetFieldValue(string fieldName, string fieldValue, string fieldType)
        {
            switch (fieldType)
            {
                case "optionset":
                    XrmApp.BusinessProcessFlow.SetValue(new OptionSet()
                    {
                        Name = fieldName,
                        Value = fieldValue,
                    });
                    break;
                case "boolean":
                    XrmApp.BusinessProcessFlow.SetValue(new BooleanItem()
                    {
                        Name = fieldName,
                        Value = bool.Parse(fieldValue),
                    });
                    break;
                case "datetime":
                    XrmApp.BusinessProcessFlow.SetValue(fieldName, DateTime.Parse(fieldValue, CultureInfo.CurrentCulture));
                    break;
                case "lookup":
                    XrmApp.BusinessProcessFlow.SetValue(new LookupItem()
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
