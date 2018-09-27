using System.IO;
using System.Reflection;
using Capgemini.Test.Xrm.Utilities.Core;
using OpenQA.Selenium;

namespace Capgemini.Test.Xrm.Utilities
{
    /// <summary>
    /// Assists in test setup.
    /// </summary>
    public class TestDriver : ITestDriver
    {
        private const string SpecXrmDriverScript = "cap.specxrm.driver.js";
        private const string SpecXrmLoadTestDataEvent = "specXrm.loadTestDataRequested";
        private const string SpecXrmDeleteTestDataEvent = "specXrm.deleteTestDataRequested";
        private const string SpecXrmOpenTestRecordEvent = "specXrm.openRecordRequested";

        private string _path;
        private string FilePath => _path ?? (_path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SpecXrmDriverScript));


        private readonly IJavaScriptExecutor _javascriptExecutor;

        /// <summary>
        /// Constructs a new TestDriver. Injects the cap.specxrm.driver.js library using the provided <see cref="IJavaScriptExecutor"/>.
        /// </summary>
        /// <param name="javascriptExecutor">The IJavaScriptExecutor to use.</param>
        public TestDriver(IJavaScriptExecutor javascriptExecutor)
        {
            _javascriptExecutor = javascriptExecutor;

            Initialise();
        }

        /// <inheritdoc cref="ITestDriver"/>
        public void LoadTestData(string data)
        {
            DispatchEvent(SpecXrmLoadTestDataEvent, data);
        }

        /// <inheritdoc cref="ITestDriver"/>
        public void DeleteTestData()
        {
            DispatchEvent(SpecXrmDeleteTestDataEvent);
        }

        /// <inheritdoc cref="ITestDriver"/>
        public void OpenTestRecord(string alias)
        {
            DispatchEvent(SpecXrmOpenTestRecordEvent, alias);
        }

        private void Initialise()
        {
            _javascriptExecutor.ExecuteScript(
                $"{File.ReadAllText(FilePath)}\n" +
                $"var testDriver = new Capgemini.Dynamics.Testing.TestDriver();");
        }

        private void DispatchEvent(string eventName, string data = "")
        {
            _javascriptExecutor.ExecuteAsyncScript(
                $@"top.dispatchEvent(
                    new CustomEvent(
                        '{eventName}', 
                        {{ 
                            detail: {{
                                data: `{data}`, 
                                callback: arguments[arguments.length - 1]
                            }}
                        }}));");
        }
    }
}
