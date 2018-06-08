using Capgemini.Test.Xrm.Configuration;
using OpenQA.Selenium;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Capgemini.Test.Xrm.Bindings.Core
{
    /// <summary>
    /// Base class for classes containing SpecFlow step definitions common across all clients.
    /// </summary>
    public abstract class XrmStepDefiner
    {
        private static XrmTestConfiguration xrmTestConfig;
        /// <summary>
        /// The configuration for the test project.
        /// </summary>
        protected static XrmTestConfiguration XrmTestConfig
        {
            get
            {
                if (xrmTestConfig == null)
                {
                    var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "xrm.test.config");
                    using (var fs = File.OpenRead(configPath))
                    {
                        xrmTestConfig = new XmlSerializer(
                            typeof(XrmTestConfiguration),
                            new[] {
                                typeof(XrmAppConfiguration),
                                typeof(XrmUserConfiguration)
                            }).Deserialize(fs) as XrmTestConfiguration;
                    }
                }
                return xrmTestConfig;
            }
        }

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
