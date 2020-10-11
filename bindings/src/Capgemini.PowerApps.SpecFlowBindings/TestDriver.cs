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
        private const string DriverScriptPath = "specflow.driver.js";
        private const string TestDriverReference = "top.testDriver";
        private const string ErrorPrefix = "driver encountered an error";

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

        private string FilePath => this.path ?? (this.path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DriverScriptPath));

        /// <inheritdoc cref="ITestDriver"/>
        public void LoadTestData(string data)
        {
            this.ExecuteAsyncScriptWithExceptionOnReject($"loadTestData(`{data}`)");
        }

        /// <inheritdoc cref="ITestDriver"/>
        public void DeleteTestData()
        {
            this.ExecuteAsyncScriptWithExceptionOnReject("deleteTestData()");
        }

        /// <inheritdoc cref="ITestDriver"/>
        public void OpenTestRecord(string recordAlias)
        {
            this.ExecuteAsyncScriptWithExceptionOnReject($"openTestRecord('{recordAlias}')");
        }

        /// <inheritdoc/>
        public EntityReference GetTestRecordReference(string recordAlias)
        {
            var obj = (Dictionary<string, object>)this.javascriptExecutor.ExecuteAsyncScript($"{TestDriverReference}.getRecordReference('{recordAlias}').then(arguments[arguments.length - 1]);");

            return new EntityReference((string)obj["entityType"], Guid.Parse((string)obj["id"]));
        }

        private static string GetExecuteScriptForAsyncFunction(string functionCall)
        {
            return $"{TestDriverReference}.{functionCall}.then(arguments[arguments.length - 1]).catch(e => {{ arguments[arguments.length - 1](`{ErrorPrefix}: ${{ e.message }}`); }});";
        }

        private object ExecuteAsyncScriptWithExceptionOnReject(string functionCall)
        {
            var result = this.javascriptExecutor.ExecuteAsyncScript(GetExecuteScriptForAsyncFunction(functionCall));

            if (result is string str && str.StartsWith(ErrorPrefix, StringComparison.InvariantCulture))
            {
                throw new WebDriverException(str);
            }

            return result;
        }

        private void Initialise()
        {
            this.javascriptExecutor.ExecuteScript(
                $"{File.ReadAllText(this.FilePath)}\n" +
                $"top.testDriver = new Capgemini.Dynamics.Testing.TestDriver();");
        }
    }
}
