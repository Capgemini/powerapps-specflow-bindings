using Capgemini.Test.Xrm.Web.Core;
using Microsoft.Dynamics365.UIAutomation.Api;
using OpenQA.Selenium;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Capgemini.Test.Xrm.Web.Steps.When
{
    /// <summary>
    /// Interaction step bindings for forms.
    /// </summary>
    [Binding]
    public class EntitySteps : XrmWebStepDefiner
    {
        /// <summary>
        /// Selects a form.
        /// </summary>
        /// <param name="form">The name of the form to select.</param>
        [When(@"I open the ""(.*)"" form")]
        public void WhenIOpenTheForm(string form)
        {
            Browser.Entity.SelectForm(form);
        }

        /// <summary>
        /// Clicks a dialog button.
        /// </summary>
        /// <param name="button">The title of the dialog button to click.</param>
        [When(@"I click the ""(.*)"" dialog button")]
        public void WhenIClickTheDialogButton(string button)
        {
            Browser.Entity.SwitchToDialogFrame();
            var buttonId = "button[title=\"" + button + "\"]";
            Browser.Driver.FindElement(By.CssSelector(buttonId)).Click();
            Browser.Entity.SwitchToContentFrame();
        }

        /// <summary>
        /// Enters a value in a text field.
        /// </summary>
        /// <param name="value">The value to enter.</param>
        /// <param name="logicalName">The logical name of the field.</param>
        [When(@"I enter ""(.*)"" in the ""(.*)"" field")]
        public void WhenIEnterInTheField(string value, string logicalName)
        {
            Browser.Entity.SetValue(logicalName, value);
        }

        /// <summary>
        /// Enters a value in a lookup field.
        /// </summary>
        /// <param name="value">The value to enter.</param>
        /// <param name="logicalName">The logical name of the field.</param>
        [When(@"I enter ""(.*)"" in the ""(.*)"" lookup field")]
        public void WhenIEnterInTheLookupField(string value, string logicalName)
        {
            Browser.Entity.SetValue(new LookupItem { Name = logicalName, Value = value });
        }

        /// <summary>
        /// Enters a value in an option set field.
        /// </summary>
        /// <param name="value">The value to enter.</param>
        /// <param name="logicalName">The logical name of the field.</param>
        [When(@"I enter ""(.*)"" in the ""(.*)"" option set field")]
        public void WhenIEnterInTheOptionSetField(string value, string logicalName)
        {
            Browser.Entity.SetValue(new OptionSet { Name = logicalName, Value = value });
        }

        /// <summary>
        /// Enters a value in a composite field.
        /// </summary>
        /// <param name="logicalName">The logical name of the composite field.</param>
        /// <param name="fields">A set of fields <see cref="Field"/></param>
        [When(@"I enter the following in the (.*) composite field")]
        public void WhenIEntertheFollowingInTheCompositeField(string logicalName, Table fields)
        {
            Browser.Entity.SetValue(new CompositeControl { Id = logicalName, Fields = new List<Field>(fields.CreateSet<Field>()) });
        }

        /// <summary>
        /// Tabs out of a field.
        /// </summary>
        /// <param name="logicalName">The logical name of the field.</param>
        [When(@"I tab out of the (.*) field")]
        public void WhenITabOutOfTheField(string logicalName)
        {
            Browser.Entity.Browser.Driver.FindElement(By.Id($"{logicalName}_i")).SendKeys(Keys.Tab);
        }

        /// <summary>
        /// Enters values in multiple fields.
        /// </summary>
        /// <param name="table">A table containing 'Field' and 'Value' columns.</param>
        [When(@"I enter these values in the following fields")]
        public void WhenIEnterTheseValuesInTheFollowingFields(Table table)
        {
            foreach (var row in table.Rows)
            {
                Browser.Entity.SetValue(row["Field"], row["Value"]);
            }
        }

        /// <summary>
        /// Enters values in multiple option set fields.
        /// </summary>
        /// <param name="table">A table containing 'Field' and 'Value' columns.</param>
        [When(@"I enter these values in the following option set fields")]
        public void WhenIEnterTheseValuesInTheFollowingOptionSetFields(Table table)
        {
            foreach (var row in table.Rows)
            {
                Browser.Entity.SetValue(new OptionSet { Name = row["Field"], Value = row["Value"] });
            }
        }

        /// <summary>
        /// Enters values in multiple lookup fields.
        /// </summary>
        /// <param name="table">A table containing 'Field' and 'Value' columns.</param>
        [When(@"I enter these values in the following lookup fields")]
        public void WhenIEnterTheseValuesInTheFollowingLookupFields(Table table)
        {
            foreach (var row in table.Rows)
            {
                Browser.Entity.SetValue(new LookupItem { Name = row["Field"], Value = row["Value"] });
            }
        }

        /// <summary>
        /// Saves the record.
        /// </summary>
        [When(@"I save the record")]
        public void WhenISaveTheRecord()
        {
            Browser.Entity.Save();
        }
    }
}
