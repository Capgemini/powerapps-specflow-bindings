using Capgemini.Test.Xrm.Bindings.Core;
using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Capgemini.Test.Xrm.Bindings.Web.Definitions
{
    /// <summary>
    /// Step definitions for user interactions within forms.
    /// </summary>
    [Binding]
    public class FormSteps : XrmWebStepDefiner
    {
        #region Given
        [Given(@"I am viewing the ""(.*)"" form")]
        public void GivenIAmViewingTheForm(string formName)
        {
            Browser.Entity.SelectForm(formName);
        }
        #endregion

        #region When
        [When(@"I click the ""(.*)"" dialog button")]
        public void WhenIClickTheDialogButton(string buttonName)
        {
            Browser.Entity.SwitchToDialogFrame();
            var buttonID = "button[title=\"" + buttonName + "\"]";
            Browser.Driver.FindElement(By.CssSelector(buttonID)).Click();
            Browser.Entity.SwitchToContentFrame();
        }

        [When(@"I enter ""(.*)"" in the ""(.*)"" field")]
        public void WhenIEnterInTheField(string value, string field)
        {
            Browser.Entity.SetValue(field, value);
        }

        [When(@"I enter ""(.*)"" in the ""(.*)"" lookup field")]
        public void WhenIEnterInTheLookupField(string value, string field)
        {
            Browser.Entity.SetValue(new LookupItem { Name = field, Value = value });
        }

        [When(@"I enter ""(.*)"" in the ""(.*)"" option set field")]
        public void WhenIEnterInTheOptionSetField(string value, string field)
        {
            Browser.Entity.SetValue(new OptionSet { Name = field, Value = value });
        }

        [When(@"I enter the following in the ""(.*)"" composite field")]
        public void WhenIEnterInTheOptionSetField(string field, Table fields)
        {
            Browser.Entity.SetValue(new CompositeControl { Id = field, Fields = new List<Field>(fields.CreateSet<Field>()) });
        }

        [When(@"I tab out of the ""(.*)"" field")]
        public void WhenITabOutOfTheField(string field)
        {
            Browser.Entity.Browser.Driver.FindElement(By.Id($"{field}_i")).SendKeys(Keys.Tab);
        }

        [When(@"I enter these values in the following fields")]
        public void WhenIEnterTheseValuesInTheFolowingFields(Table table)
        {
            foreach (var row in table.Rows)
                Browser.Entity.SetValue(row["Field"], row["Value"]);
        }

        [When(@"I save the record")]
        public void WhenISaveTheRecord()
        {
            Browser.Entity.Save();
        }
        #endregion

        #region Then
        [Then(@"I should see the ""(.*)"" field")]
        public void ThenIShouldSeeTheField(string field)
        {
            Assert.IsTrue(Browser.Entity.Browser.Driver.IsVisible(By.Id(field)));
        }

        [Then(@"I should see an error on the ""(.*)"" field which reads ""(.*)""")]
        public void ThenIShouldSeeAnErrorOnTheFieldWhichReads(string field, string message)
        {
            var errors = Browser.Entity.Browser.Driver.FindElements(By.CssSelector($"#{field} > div.ms-crm-Inline-Validation"));

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(message, errors[0].Text);
            Assert.IsTrue(errors[0].Displayed);
        }

        [Then(@"I should see the following fields")]
        public void ThenIShouldSeeTheFollowingFields(Table table)
        {
            foreach (var row in table.Rows)
            {
                Assert.IsTrue(Browser.Entity.Browser.Driver.IsVisible(By.Id(row["Field"])));
            }
        }
        #endregion
    }
}
