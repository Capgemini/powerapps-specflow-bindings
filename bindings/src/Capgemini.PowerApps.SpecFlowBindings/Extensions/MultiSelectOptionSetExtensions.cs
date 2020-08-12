namespace Capgemini.PowerApps.SpecFlowBindings.Extensions
{
    using System;
    using System.Linq;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI.DTO;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using OpenQA.Selenium;

    /// <summary>
    /// Extensions for EasyRepro classes to fix MultiSelectOptionSets.
    /// </summary>
    public static class MultiSelectOptionSetExtensions
    {
        /// <summary>
        /// Sets the MultiSelectOptionSet field value.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="driver">The Selenium WebDriver.</param>
        /// <param name="option">The multiselectoptionset.</param>
        /// <param name="removeExistingValues">Controls whether currently selected values should be removed first.</param>
        public static void SetMultiSelectOptionSetValue(this Entity entity, IWebDriver driver, MultiValueOptionSet option, bool removeExistingValues = false)
        {
            SetMultiSelectOptionSetValue(driver, option, FormContextType.Entity, removeExistingValues);
        }

        /// <summary>
        /// Sets the MultiSelectOptionSet field value.
        /// </summary>
        /// <param name="quickCreate">The QuickCreate.</param>
        /// <param name="driver">The Selenium WebDriver.</param>
        /// <param name="option">The multiselectoptionset.</param>
        /// <param name="removeExistingValues">Controls whether currently selected values should be removed first.</param>
        public static void SetMultiSelectOptionSetValue(this QuickCreate quickCreate, IWebDriver driver, MultiValueOptionSet option, bool removeExistingValues = false)
        {
            SetMultiSelectOptionSetValue(driver, option, FormContextType.QuickCreate, removeExistingValues);
        }

        /// <summary>
        /// Sets the MultiSelectOptionSet field value.
        /// </summary>
        /// <param name="businessProcessFlow">The BusinessProcessFlow.</param>
        /// <param name="driver">The Selenium WebDriver.</param>
        /// <param name="option">The multiselectoptionset.</param>
        /// <param name="removeExistingValues">Controls whether currently selected values should be removed first.</param>
        public static void SetMultiSelectOptionSetValue(this BusinessProcessFlow businessProcessFlow, IWebDriver driver, MultiValueOptionSet option, bool removeExistingValues = false)
        {
            SetMultiSelectOptionSetValue(driver, option, FormContextType.BusinessProcessFlow, removeExistingValues);
        }

        private static void SetMultiSelectOptionSetValue(IWebDriver driver, MultiValueOptionSet option, FormContextType formContextType, bool removeExistingValues = false)
        {
            driver = driver ?? throw new ArgumentNullException(nameof(driver));
            option = option ?? throw new ArgumentNullException(nameof(option));

            if (removeExistingValues)
            {
                RemoveMultiOptions(driver, option, formContextType);
            }

            AddMultiOptions(driver, option, formContextType);
        }

        private static void RemoveMultiOptions(IWebDriver driver, MultiValueOptionSet option, FormContextType formContextType)
        {
            IWebElement fieldContainer = GetMultiSelectOptionSetFieldContainer(driver, option, formContextType);

            fieldContainer.Click();
            fieldContainer.FindElement(By.XPath(".//div[@class=\"msos-caret-container\"]")).Click();

            var selectedItems = fieldContainer.FindElements(By.XPath(".//li[contains(@class, \"msos-option-selected\")]"));

            foreach (IWebElement item in selectedItems)
            {
                item.Click();
            }
        }

        private static void AddMultiOptions(IWebDriver driver, MultiValueOptionSet option, FormContextType formContextType)
        {
            var fieldContainer = GetMultiSelectOptionSetFieldContainer(driver, option, formContextType);

            fieldContainer.Click();

            foreach (var optionValue in option.Values)
            {
                var input = fieldContainer.FindElement(By.TagName("input"));
                input.Clear();
                input.SendKeys(optionValue);

                var searchFlyout = fieldContainer.WaitUntilAvailable(By.XPath(".//div[contains(@class,\"msos-selection-container\")]//ul"));

                var searchResultList = searchFlyout.FindElements(By.XPath(".//li//label[@name=\"[NAME]msos-label\"]".Replace("[NAME]", option.Name)));

                if (searchResultList.Any(x => x.GetAttribute("title").Contains(optionValue, StringComparison.OrdinalIgnoreCase)))
                {
                    searchResultList.FirstOrDefault(x => x.GetAttribute("title").Contains(optionValue, StringComparison.OrdinalIgnoreCase)).Click(true);
                    driver.WaitForTransaction();
                }
                else
                {
                    throw new InvalidOperationException($"Option with text '{optionValue}' could not be found for '{option.Name}'");
                }
            }

            fieldContainer.FindElement(By.XPath(".//div[@class=\"msos-caret-container\"]"))
                .Click();
        }

        private static IWebElement GetMultiSelectOptionSetFieldContainer(IWebDriver driver, MultiValueOptionSet option, FormContextType formContextType)
        {
            IWebElement formContext;
            switch (formContextType)
            {
                case FormContextType.QuickCreate:
                    formContext = driver.WaitUntilAvailable(By.XPath(AppElements.Xpath[AppReference.QuickCreate.QuickCreateFormContext]));
                    return formContext.WaitUntilAvailable(By.XPath(AppElements.Xpath[AppReference.MultiSelect.DivContainer].Replace("[NAME]", option.Name)));
                case FormContextType.Entity:
                    formContext = driver.WaitUntilAvailable(By.XPath(AppElements.Xpath[AppReference.Entity.FormContext]));
                    return formContext.WaitUntilAvailable(By.XPath(AppElements.Xpath[AppReference.MultiSelect.DivContainer].Replace("[NAME]", option.Name)));
                case FormContextType.BusinessProcessFlow:
                    formContext = driver.WaitUntilAvailable(By.XPath(AppElements.Xpath[AppReference.BusinessProcessFlow.BusinessProcessFlowFormContext]));
                    return formContext.WaitUntilAvailable(By.XPath(AppElements.Xpath[AppReference.MultiSelect.DivContainer].Replace("[NAME]", option.Name)));
                default:
                    throw new Exception($"Mapping for FormContextType {formContextType} not configured.");
            }
        }
    }
}
