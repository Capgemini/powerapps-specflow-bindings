namespace Capgemini.PowerApps.SpecFlowBindings.Extensions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using OpenQA.Selenium;

    /// <summary>
    /// Extensions to the <see cref="Field"/> class.
    /// </summary>
    public static class FieldExtensions
    {
        /// <summary>
        /// This is required as the <see cref="Field.IsReadOnly"/> property does not work.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="driver">The Selenium WebDriver.</param>
        /// <returns>Whether the field is read-only.</returns>
        public static bool IsReadOnly(this Field field, IWebDriver driver)
        {
            field = field ?? throw new ArgumentNullException(nameof(field));
            driver = driver ?? throw new ArgumentNullException(nameof(driver));

            ReadOnlyCollection<IWebElement> lockedIcons = driver.FindElements(By.CssSelector($"div[data-id={field.Name}-locked-iconWrapper]"));

            throw new Exception("no detail");
            return lockedIcons.Any();
        }
    }
}
