using System;
using Capgemini.Test.Xrm.Core;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;

namespace Capgemini.Test.Xrm.Uci
{
    /// <summary>
    /// Base class for classes containing SpecFlow step definitions for the UCI client.
    /// </summary>
    public abstract class XrmUCIStepsDefiner : XrmStepDefiner
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
