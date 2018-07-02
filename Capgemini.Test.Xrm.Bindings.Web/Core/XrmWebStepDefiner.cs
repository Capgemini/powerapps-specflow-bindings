using System;
using Capgemini.Test.Xrm.Bindings.Core;
using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;

namespace Capgemini.Test.Xrm.Bindings.Web.Core
{
    /// <summary>
    /// Base class for classes containing SpecFlow step definitions for the web client.
    /// </summary>
    public abstract class XrmWebStepDefiner : XrmStepDefiner
    {
        [ThreadStatic]
        private static Browser _browser;
        /// <summary>
        /// EasyRepro Browser.
        /// </summary>
        protected static Browser Browser => _browser ?? (_browser = new Browser(
                                                new BrowserOptions
                                                {
                                                    BrowserType = BrowserType.Chrome,
                                                    PrivateMode = true
                                                }));

        /// <inheritdoc />
        protected override IWebDriver Driver => Browser.Driver;

        /// <inheritdoc />
        protected sealed override void Quit()
        {
            if (_browser == null) return;

            _browser.Dispose();
            _browser = null;
        }
    }
}
