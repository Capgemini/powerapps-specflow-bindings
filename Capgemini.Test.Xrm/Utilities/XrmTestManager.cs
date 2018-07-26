using System.IO;
using System.Reflection;
using Capgemini.Test.Xrm.Utilities.Core;
using OpenQA.Selenium;

namespace Capgemini.Test.Xrm.Utilities
{
    /// <summary>
    /// Wrapper for the cap.xrm.test.manager.js library.
    /// </summary>
    public class XrmTestManager : ITestUtility
    {
        private const string FileName = "cap.xrm.test.manager.js";
        private const string GlobalVariable = "window.top.xrmTestManager";
        private const string Constructor = "Capgemini.Dynamics.Testing.XrmTestManager";
        private const string CallbackFunction = "arguments[arguments.length - 1]";

        private string _path;
        private string FilePath => _path ?? (_path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FileName));

        private static class Methods
        {
            public const string OpenTestRecord = "openTestRecord";
            public const string LoadTestData = "loadTestData";
            public const string DeleteTestData = "deleteTestData";
        }

        private readonly IJavaScriptExecutor _javascriptExecutor;

        /// <summary>
        /// Constructs a new XrmTestManager.
        /// Calling this constructor will inject the cap.xrm.test.manager.js library using the provided IJavaScriptExecutor.
        /// </summary>
        /// <param name="javascriptExecutor">The IJavaScriptExecutor to use.</param>
        public XrmTestManager(IJavaScriptExecutor javascriptExecutor)
        {
            _javascriptExecutor = javascriptExecutor;

            Inject();
        }

        /// <inheritdoc cref="ITestUtility"/>
        public void LoadTestData(string data)
        {
            _javascriptExecutor.ExecuteAsyncScript($"{GlobalVariable}.{Methods.LoadTestData}(JSON.stringify({data})).then({CallbackFunction});");
        }

        /// <inheritdoc cref="ITestUtility"/>
        public void DeleteTestData()
        {
            _javascriptExecutor.ExecuteAsyncScript(
                $"{GlobalVariable}.{Methods.DeleteTestData}().then({CallbackFunction});");
        }

        /// <inheritdoc cref="ITestUtility"/>
        public void OpenTestRecord(string alias)
        {
            _javascriptExecutor.ExecuteAsyncScript(
                $"{GlobalVariable}.{Methods.OpenTestRecord}(\"{alias}\").then({CallbackFunction});");
        }

        private void Inject()
        {
            _javascriptExecutor.ExecuteScript(
                $"{File.ReadAllText(FilePath)}" +
                $"{GlobalVariable} = new {Constructor}()");
        }
    }
}
