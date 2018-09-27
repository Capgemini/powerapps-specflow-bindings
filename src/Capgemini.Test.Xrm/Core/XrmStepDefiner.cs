using System;
using Capgemini.Test.Xrm.Configuration;
using OpenQA.Selenium;
using System.IO;
using System.Reflection;
using Capgemini.Test.Xrm.Data;
using Capgemini.Test.Xrm.Data.Core;
using Capgemini.Test.Xrm.Utilities;
using Capgemini.Test.Xrm.Utilities.Core;
using YamlDotNet.Serialization;

namespace Capgemini.Test.Xrm.Core
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
                    var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), XrmTestConfiguration.FileName);
                    _xrmTestConfig = new Deserializer().Deserialize<XrmTestConfiguration>(File.ReadAllText(configPath));
                }
                return _xrmTestConfig;
            }
        }

        [ThreadStatic]
        private static ITestDriver _testDriver;
        /// <summary>
        /// Provides utilities for test setup/teardown.
        /// </summary>
        protected ITestDriver TestDriver => _testDriver ?? (_testDriver = new TestDriver((IJavaScriptExecutor)Driver));

        [ThreadStatic]
        private static ITestDataRepository _testDataRepository;
        /// <summary>
        /// Provides access to test data.
        /// </summary>
        protected ITestDataRepository TestDataRepository => _testDataRepository ?? (_testDataRepository = new TestDataRepository());

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
