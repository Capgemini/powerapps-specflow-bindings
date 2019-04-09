using System;
using Capgemini.Test.Xrm.Core;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;

namespace Capgemini.Test.Xrm.Uci
{
    /// <summary>
    /// Base class for defining step bindings for the UCI client.
    /// </summary>
    public abstract class XrmUciStepDefiner : XrmStepDefiner
    {
        [ThreadStatic]
        private static WebClient _client;

        /// <summary>
        /// EasyRepro WebClient.
        /// </summary>
        protected static WebClient Client => _client ?? (_client = new WebClient(
                                                 new BrowserOptions
                                                 {
                                                     BrowserType = BrowserType.Chrome,
                                                     PrivateMode = true
                                                 }));

        [ThreadStatic]
        private static XrmApp _xrmApp;

        /// <summary>
        /// EasyRepro XrmApp.
        /// </summary>
        protected static XrmApp XrmApp => _xrmApp ?? (_xrmApp = new XrmApp(Client));

        /// <inheritdoc />
        protected override IWebDriver Driver => Client.Browser.Driver;

        private BrowserOptions GetBrowserOptions()
        {
            var options = new BrowserOptions
            {
                UserAgent = true,
                UserAgentValue = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36",
                BrowserType = (BrowserType)Enum.Parse(typeof(BrowserType), XrmTestConfig.Browser.ToString()),
                // TODO: merge remote server URL support into 9.1 branch of EasyRepro
                // RemoteServerUrl = string.IsNullOrEmpty(XrmTestConfig.RemoteServerUrl) ? null : new Uri(XrmTestConfig.RemoteServerUrl),
            };
            return options;
        }

        /// <inheritdoc />
        protected sealed override void Quit()
        {
            if (_xrmApp == null) return;

            _xrmApp.Dispose();
            _xrmApp = null;
            _testDriver = null;
        }
    }
}
