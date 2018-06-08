using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;

namespace Capgemini.Test.Xrm.Bindings.Core
{
    /// <summary>
    /// Base class for classes containing SpecFlow step definitions for the web client.
    /// </summary>
    public abstract class XrmWebStepDefiner : XrmStepDefiner
    {
        private static Browser browser;
        /// <summary>
        /// EasyRepro Browser.
        /// </summary>
        protected static Browser Browser
        {
            get
            {
                if (browser == null)
                {
                    browser = new Browser(
                            new BrowserOptions
                            {
                                BrowserType = BrowserType.Chrome,
                                PrivateMode = true
                            });
                }
                return browser;
            }
        }

        /// <summary>
        /// Browser WebDriver.
        /// </summary>
        protected override IWebDriver Driver
        {
            get
            {
                return Browser.Driver;
            }
        }

        protected sealed override void Quit()
        {
            if (browser != null)
            {
                browser.Dispose();
                browser = null;
            }
        }
    }
}
