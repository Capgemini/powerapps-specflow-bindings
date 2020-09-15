namespace Capgemini.PowerApps.SpecFlowBindings.Extensions
{
    using System;
    using System.Linq;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using OpenQA.Selenium;

    /// <summary>
    /// Extensions to the <see cref="Grid"/> class.
    /// </summary>
    public static class GridExtensions
    {
        /// <summary>
        /// Gets the index of a record by ID.
        /// Accessing grid item IDs or URLs via EasyRepro is currently broken (see https://github.com/microsoft/EasyRepro/issues/800). Use this method instead.
        /// </summary>
        /// <param name="grid">The Grid.</param>
        /// <param name="driver">The Selenium WebDriver.</param>
        /// <param name="recordId">The ID of the record.</param>
        /// <returns>The index of the record.</returns>
        public static int GetRecordIndexById(this Grid grid, IWebDriver driver, Guid recordId)
        {
            if (driver is null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            driver.WaitUntilAvailable(By.XPath(AppElements.Xpath[AppReference.Grid.Container]));

            var index = (long)driver.ExecuteScript($"return Object.keys(getCurrentXrmStatus().mainGrid._grid.getRows()._collection).indexOf(\"{recordId}\");");
            if (index == -1)
            {
                throw new Exception($"A record with an ID of {recordId} is not present in the grid");
            }

            return Convert.ToInt32(index);
        }
    }
}
