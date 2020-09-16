namespace Capgemini.PowerApps.SpecFlowBindings
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Microsoft.Xrm.Sdk;
    using OpenQA.Selenium;

    /// <summary>
    /// Assists in test setup.
    /// </summary>
    public class TestDriver : ITestDriver
    {
        private const string DriveScriptPath = "specflow.driver.js";
        private const string LoadTestDataEvent = "driver.loadTestDataRequested";
        private const string DeleteTestDataEvent = "driver.deleteTestDataRequested";
        private const string OpenTestRecordEvent = "driver.openRecordRequested";
        private const string GetRecordReferenceEvent = "driver.getRecordReferenceRequested";

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

        private string FilePath => this.path ?? (this.path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DriveScriptPath));

        /// <inheritdoc cref="ITestDriver"/>
        public void LoadTestData(string data)
        {
            this.DispatchEvent(LoadTestDataEvent, data);
        }

        /// <inheritdoc cref="ITestDriver"/>
        public void DeleteTestData()
        {
            this.DispatchEvent(DeleteTestDataEvent);
        }

        /// <inheritdoc cref="ITestDriver"/>
        public void OpenTestRecord(string recordAlias)
        {
            this.DispatchEvent(OpenTestRecordEvent, recordAlias);
        }

        /// <inheritdoc/>
        public EntityReference GetTestRecordReference(string recordAlias)
        {
            var obj = (Dictionary<string, object>)this.DispatchEvent(GetRecordReferenceEvent, recordAlias);
            return new EntityReference((string)obj["entityType"], Guid.Parse((string)obj["id"]));
        }

        private void Initialise()
        {
            this.javascriptExecutor.ExecuteScript(
                $"{File.ReadAllText(this.FilePath)}\n" +
                $"top.testDriver = new Capgemini.Dynamics.Testing.TestDriver();");
        }

        private object DispatchEvent(string eventName, string data = "")
        {
            try
            {
                return this.javascriptExecutor.ExecuteAsyncScript(
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
            catch (WebDriverTimeoutException ex)
            {
                throw new Exception($"The test driver script errored while handling the {eventName} event", ex);
            }
        }
    }
}
