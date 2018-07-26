using System;
using Capgemini.Test.Xrm.Configuration;
using OpenQA.Selenium;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Capgemini.Test.Xrm.Data;
using Capgemini.Test.Xrm.Data.Core;
using Capgemini.Test.Xrm.Utilities;
using Capgemini.Test.Xrm.Utilities.Core;

namespace Capgemini.Test.Xrm.Bindings.Core
{
    /// <summary>
    /// Base class for classes containing SpecFlow step definitions common across all clients.
    /// </summary>
    public abstract class XrmStepDefiner
    {
        private static XrmTestConfiguration _xrmTestConfig;
        /// <summary>
        /// The configuration for the test project.
        /// </summary>
        protected XrmTestConfiguration XrmTestConfig
        {
            get
            {
                if (_xrmTestConfig == null)
                {
                    var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "xrm.test.config");
                    using (var fs = File.OpenRead(configPath))
                    {
                        _xrmTestConfig = new XmlSerializer(
                            typeof(XrmTestConfiguration),
                            new[] {
                                typeof(XrmAppConfiguration),
                                typeof(XrmUserConfiguration)
                            }).Deserialize(fs) as XrmTestConfiguration;
                    }
                }
                return _xrmTestConfig;
            }
        }

        [ThreadStatic]
        private static ITestUtility _utility;
        /// <summary>
        /// Provides utilities for test setup/teardown.
        /// </summary>
        protected ITestUtility Utility => _utility ?? (_utility = new XrmTestManager((IJavaScriptExecutor)Driver));

        [ThreadStatic]
        private static ITestDataRepository _testDataRepository;
        /// <summary>
        /// Provides access to test data.
        /// </summary>
        protected ITestDataRepository TestDataRepository => _testDataRepository ?? (_testDataRepository = new FileTestDataRepository());

        /// <summary>
        /// The Selenium WebDriver.
        /// </summary>
        protected abstract IWebDriver Driver { get; }

        /// <summary>
        /// Performs any cleanup necessary when quitting the WebBrowser
        /// </summary>
        protected abstract void Quit();
    }
}
