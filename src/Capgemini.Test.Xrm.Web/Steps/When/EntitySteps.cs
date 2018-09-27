using Capgemini.Test.Xrm.Web.Core;
using Microsoft.Dynamics365.UIAutomation.Api;
using OpenQA.Selenium;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Capgemini.Test.Xrm.Web.Steps.When
{
    [Binding]
    public class EntitySteps : XrmWebStepDefiner
    {


        [When(@"I open the ""(.*)"" form")]
        public void WhenIOpenTheForm(string form)
        {
            Browser.Entity.SelectForm(form);
        }

        [When(@"I click the ""(.*)"" dialog button")]
        public void WhenIClickTheDialogButton(string button)
        {
            Browser.Entity.SwitchToDialogFrame();
            var buttonId = "button[title=\"" + button + "\"]";
            Browser.Driver.FindElement(By.CssSelector(buttonId)).Click();
            Browser.Entity.SwitchToContentFrame();
        }

        [When(@"I enter ""(.*)"" in the (.*) field")]
        public void WhenIEnterInTheField(string value, string logicalName)
        {
            Browser.Entity.SetValue(logicalName, value);
        }

        [When(@"I enter ""(.*)"" in the (.*) lookup field")]
        public void WhenIEnterInTheLookupField(string value, string logicalName)
        {
            Browser.Entity.SetValue(new LookupItem { Name = logicalName, Value = value });
        }

        [When(@"I enter ""(.*)"" in the (.*) option set field")]
        public void WhenIEnterInTheOptionSetField(string value, string logicalName)
        {
            Browser.Entity.SetValue(new OptionSet { Name = logicalName, Value = value });
        }

        [When(@"I enter the following in the (.*) composite field")]
        public void WhenIEntertheFollowingInTheCompositeField(string logicalName, Table fields)
        {
            Browser.Entity.SetValue(new CompositeControl { Id = logicalName, Fields = new List<Field>(fields.CreateSet<Field>()) });
        }

        [When(@"I tab out of the (.*) field")]
        public void WhenITabOutOfTheField(string logicalName)
        {
            Browser.Entity.Browser.Driver.FindElement(By.Id($"{logicalName}_i")).SendKeys(Keys.Tab);
        }

        [When(@"I enter these values in the following fields")]
        public void WhenIEnterTheseValuesInTheFollowingFields(Table table)
        {
            foreach (var row in table.Rows)
            {
                Browser.Entity.SetValue(row["Field"], row["Value"]);
            }
        }

        [When(@"I enter these values in the following option set fields")]
        public void WhenIEnterTheseValuesInTheFollowingOptionSetFields(Table table)
        {
            foreach (var row in table.Rows)
            {
                Browser.Entity.SetValue(new OptionSet { Name = row["Field"], Value = row["Value"] });
            }
        }

        [When(@"I enter these values in the following lookup fields")]
        public void WhenIEnterTheseValuesInTheFollowingLookupFields(Table table)
        {
            foreach (var row in table.Rows)
            {
                Browser.Entity.SetValue(new LookupItem { Name = row["Field"], Value = row["Value"] });
            }
        }

        [When(@"I save the record")]
        public void WhenISaveTheRecord()
        {
            Browser.Entity.Save();
        }
    }
}
