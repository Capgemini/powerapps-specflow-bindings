namespace Capgemini.PowerApps.SpecFlowBindings.Extensions
{
    using System;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
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
    }
}
