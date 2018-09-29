using System;
using Capgemini.Test.Xrm.Core;
using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;

namespace Capgemini.Test.Xrm.Web.Core
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
        protected Browser Browser
        {
            get
            {
                if (_browser == null)
                {
                    _browser = new Browser(GetBrowserOptions());
                }
                return _browser;
            }
        }

        private BrowserOptions GetBrowserOptions()
        {
            var options = new BrowserOptions
            {
                UserAgent = true,
                UserAgentValue = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36",
                BrowserType = (BrowserType)Enum.Parse(typeof(BrowserType), XrmTestConfig.Browser.ToString()),
                RemoteServerUrl = string.IsNullOrEmpty(XrmTestConfig.RemoteServerUrl) ? null : new Uri(XrmTestConfig.RemoteServerUrl),
            };
            return options;
        }

        /// <inheritdoc />
        protected override IWebDriver Driver => Browser.Driver;

        /// <inheritdoc />
        protected sealed override void Quit()
        {
            if (_browser == null) return;

            _browser.Dispose();
            _browser = null;
            _testDriver = null;
        }
    }
}
