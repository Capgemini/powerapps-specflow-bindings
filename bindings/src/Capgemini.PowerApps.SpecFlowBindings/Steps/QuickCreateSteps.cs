namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using System;
    using System.Globalization;
    using Capgemini.PowerApps.SpecFlowBindings.Extensions;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Step bindings related to quick creates.
    /// </summary>
    [Binding]
    public class QuickCreateSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Selects a record by name in a quick create lookup.
        /// </summary>
        /// <param name="name">The name of the record.</param>
        /// <param name="lookup">The lookup logical name.</param>
        [When(@"I select a record named '(.*)' in the '(.*)' lookup on the quick create")]
        public static void WhenISelectARecordNamedInTheLookupOnTheQuickCreate(string name, string lookup)
        {
            XrmApp.QuickCreate.SetValue(new LookupItem { Name = lookup, Value = name });
        }

        /// <summary>
        /// Sets the value for the field on the quick create.
        /// </summary>
        /// <param name="fieldValue">The Field Value.</param>
        /// <param name="fieldName">The Field Name.</param>
        /// <param name="fieldType">The Field Type.</param>
        [When(@"I enter '(.*)' into the '(.*)' (text|optionset|boolean|numeric|currency|datetime|lookup) field on the quick create")]
        public static void WhenIEnterInTheFieldOnTheQuickCreate(string fieldValue, string fieldName, string fieldType)
        {
            SetFieldValue(fieldName, fieldValue.ReplaceTemplatedText(), fieldType);
        }

        /// <summary>
        /// Saves a quick create.
        /// </summary>
        [When(@"I save the quick create")]
        public static void WhenISaveTheQuickCreate()
        {
            XrmApp.QuickCreate.Save();
        }

        private static void SetFieldValue(string fieldName, string fieldValue, string fieldType)
        {
            switch (fieldType)
            {
                case "optionset":
                    XrmApp.QuickCreate.SetValue(new OptionSet()
                    {
                        Name = fieldName,
                        Value = fieldValue,
                    });
                    break;
                case "boolean":
                    XrmApp.QuickCreate.SetValue(new BooleanItem()
                    {
                        Name = fieldName,
                        Value = bool.Parse(fieldValue),
                    });
                    break;
                case "datetime":
                    XrmApp.QuickCreate.SetValue(fieldName, DateTime.Parse(fieldValue, CultureInfo.CreateSpecificCulture("en-GB")));
                    break;
                case "lookup":
                    XrmApp.QuickCreate.SetValue(new LookupItem()
                    {
                        Name = fieldName,
                        Value = fieldValue,
                    });
                    break;
                case "currency":
                case "numeric":
                case "text":
                default:
                    XrmApp.QuickCreate.SetValue(fieldName, fieldValue);
                    break;
            }
        }
    }
}
