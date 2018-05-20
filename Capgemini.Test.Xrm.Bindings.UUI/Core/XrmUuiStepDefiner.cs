using Capgemini.Test.Xrm.Bindings.Core;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;

namespace Capgemini.Test.Xrm.EasyRepro
{
    /// <summary>
    /// Base class for classes containing SpecFlow step definitions for the UUI client.
    /// </summary>
    public abstract class XrmUuiStepsDefiner : XrmStepDefiner
    {
        private static WebClient client;
        /// <summary>
        /// EasyRepro WebClient.
        /// </summary>
        protected static WebClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new WebClient(
                        new BrowserOptions
                        {
                            BrowserType = BrowserType.Chrome,
                            PrivateMode = true
                        });
                }
                return client;
            }
        }

        private static XrmApp xrmApp;
        /// <summary>
        /// EasyRepro XrmApp.
        /// </summary>
        protected static XrmApp XrmApp
        {
            get
            {
                if (xrmApp == null)
                {
                    xrmApp = new XrmApp(Client);
                }
                return xrmApp;
            }
        }

        /// <summary>
        /// Browser WebDriver.
        /// </summary>
        protected override IWebDriver Driver
        {
            get
            {
                return Client.Browser.Driver;
            }
        }
    }
}
