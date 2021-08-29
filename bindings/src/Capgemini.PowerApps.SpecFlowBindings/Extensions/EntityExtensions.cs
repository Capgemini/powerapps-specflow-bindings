namespace Capgemini.PowerApps.SpecFlowBindings.Extensions
{
    using System;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using OpenQA.Selenium;

    /// <summary>
    /// Extensions to the <see cref="Entity"/> class.
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// This is required as <see cref="Entity.GetField"/> throws a <see cref="NullReferenceException"/> when getting a hidden field.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="driver">The Selenium WebDriver.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <returns>Whether the field is visible.</returns>
        public static bool IsFieldVisible(this Entity entity, IWebDriver driver, string fieldName)
        {
            driver = driver ?? throw new ArgumentNullException(nameof(driver));

            var elements = driver.FindElements(By.CssSelector($"div[data-id={fieldName}]"));

            return elements.Count > 0;
        }

        /// <summary>
        /// This is required as <see cref="Entity.GetFormName"/> throws a <see cref="WebDriverException"/> when trying to get a form selector on a quick create form.
        /// </summary>
        /// <param name="driver">The Selenium WebDriver.</param>
        /// <returns>The current form name.</returns>
        public static string GetFormName(IWebDriver driver)
        {
            driver = driver ?? throw new ArgumentNullException(nameof(driver));

            if (driver.HasElement(By.XPath("//section[@data-id = 'quickCreateRoot']")))
            {
                return driver.FindElement(By.XPath("//h1[@data-id = 'quickHeaderTitle']")).Text;
            }

            driver.WaitUntilVisible(By.XPath(AppElements.Xpath[AppReference.Entity.FormSelector]));

            string formName = driver.ExecuteScript("return Xrm.Page.ui.formContext.ui.formSelector.getCurrentItem().getLabel();").ToString();

            if (string.IsNullOrEmpty(formName))
            {
                throw new NotFoundException("Unable to retrieve Form Name for this entity");
            }

            return formName;
        }
    }
}
