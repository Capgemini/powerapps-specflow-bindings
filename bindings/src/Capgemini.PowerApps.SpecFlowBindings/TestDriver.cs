namespace Capgemini.PowerApps.SpecFlowBindings
{
    using System.IO;
    using System.Reflection;
    using OpenQA.Selenium;

    /// <summary>
    /// Assists in test setup.
    /// </summary>
    public class TestDriver : ITestDriver
    {
        private const string SpecXrmDriverScript = "specflow.driver.js";
        private const string SpecXrmLoadTestDataEvent = "specXrm.loadTestDataRequested";
        private const string SpecXrmDeleteTestDataEvent = "specXrm.deleteTestDataRequested";
        private const string SpecXrmOpenTestRecordEvent = "specXrm.openRecordRequested";

        private readonly IJavaScriptExecutor javascriptExecutor;

        private string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDriver"/> class.
        /// Injects the specflow.driver.js library using the provided <see cref="IJavaScriptExecutor"/>.
        /// </summary>
        /// <param name="javascriptExecutor">The IJavaScriptExecutor to use.</param>
        public TestDriver(IJavaScriptExecutor javascriptExecutor)
        {
            this.javascriptExecutor = javascriptExecutor;

            this.Initialise();
        }

        private string FilePath => this.path ?? (this.path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SpecXrmDriverScript));

        /// <inheritdoc cref="ITestDriver"/>
        public void LoadTestData(string data)
        {
            this.DispatchEvent(SpecXrmLoadTestDataEvent, data);
        }

        /// <inheritdoc cref="ITestDriver"/>
        public void DeleteTestData()
        {
            this.DispatchEvent(SpecXrmDeleteTestDataEvent);
        }

        /// <inheritdoc cref="ITestDriver"/>
        public void OpenTestRecord(string alias)
        {
            this.DispatchEvent(SpecXrmOpenTestRecordEvent, alias);
        }

        private void Initialise()
        {
            this.javascriptExecutor.ExecuteScript(
                $"{File.ReadAllText(this.FilePath)}\n" +
                $"var testDriver = new Capgemini.Dynamics.Testing.TestDriver();");
        }

        private void DispatchEvent(string eventName, string data = "")
        {
            this.javascriptExecutor.ExecuteAsyncScript(
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
