using Capgemini.Test.Xrm.Web.Core;
using Microsoft.Dynamics365.UIAutomation.Browser;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Web.Steps.Then
{
    /// <summary>
    /// Assertion step bindings for forms.
    /// </summary>
    [Binding]
    public class EntitySteps : XrmWebStepDefiner
    {
        /// <summary>
        /// Asserts that a field is visible.
        /// </summary>
        /// <param name="field">The logical name of the field.</param>
        [Then(@"I should see the (.*) field")]
        public void ThenIShouldSeeTheField(string field)
        {
            Assert.IsTrue(Browser.Entity.Browser.Driver.IsVisible(By.Id(field)));
        }

        /// <summary>
        /// Asserts that a field is not visible.
        /// </summary>
        /// <param name="field">The logical name of the field.</param>
        [Then(@"I shouldn't see the (.*) field")]
        public void ThenIShouldntSeeTheField(string field)
        {
            Assert.IsFalse(Browser.Entity.Browser.Driver.IsVisible(By.Id(field)));
        }

        /// <summary>
        /// Asserts that an error message is visible on field.
        /// </summary>
        /// <param name="field">The logical name of the field.</param>
        /// <param name="message">The expected error message.</param>
        [Then(@"I should see an error on the (.*) field which reads ""(.*)""")]
        public void ThenIShouldSeeAnErrorOnTheFieldWhichReads(string field, string message)
        {
            var errors = Browser.Entity.Browser.Driver.FindElements(By.CssSelector($"#{field} > div.ms-crm-Inline-Validation"));

            Assert.IsTrue(errors.Count == 1 && errors[0].Text == message && errors[0].Displayed == true);
        }

        /// <summary>
        /// Asserts that multiple fields are visible.
        /// </summary>
        /// <param name="table">A table containing a 'Fields' column with the logical names of the fields.</param>
        [Then(@"I should see the following fields")]
        public void ThenIShouldSeeTheFollowingFields(Table table)
        {
            foreach (var row in table.Rows)
            {
                Assert.IsTrue(Browser.Entity.Browser.Driver.IsVisible(By.Id(row["Field"])));
            }
        }

    }
}
