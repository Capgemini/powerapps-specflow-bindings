﻿namespace Capgemini.PowerApps.SpecFlowBindings
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Microsoft.Xrm.Sdk;
    using OpenQA.Selenium;

    /// <summary>
    /// Assists in test setup.
    /// </summary>
    public class TestDriver : ITestDriver
    {
        private const string DriverScriptPath = "driver.js";
        private const string TestDriverReference = "top.driver";
        private const string ErrorPrefix = "driver encountered an error";
        private const string LibraryNamespace = "PowerAppsSpecFlowBindings";

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
        }

        private string FilePath => this.path ?? (this.path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DriverScriptPath));

        /// <inheritdoc/>
        public void InjectOnPage(string authToken)
        {
            var scriptBuilder = new StringBuilder();
            scriptBuilder.AppendLine(File.ReadAllText(this.FilePath));
            scriptBuilder.AppendLine($@"top.recordRepository = new {LibraryNamespace}.CurrentUserRecordRepository(Xrm.WebApi.online);
                   top.metadataRepository = new {LibraryNamespace}.MetadataRepository(Xrm.WebApi.online);
                   top.deepInsertService = new {LibraryNamespace}.DeepInsertService(top.metadataRepository, top.recordRepository);
                   top.recordService = new {LibraryNamespace}.RecordService(top.metadataRepository, top.recordRepository);");

            if (!string.IsNullOrEmpty(authToken))
            {
                scriptBuilder.AppendLine(
                    $@"top.appUserRecordRepository = new {LibraryNamespace}.AuthenticatedRecordRepository(top.metadataRepository, '{authToken}');
                       top.dataManager = new {LibraryNamespace}.DataManager(top.recordRepository, top.deepInsertService, top.recordService, [new {LibraryNamespace}.FakerPreprocessor()], top.appUserRecordRepository);");
            }
            else
            {
                scriptBuilder.AppendLine(
                    $"top.dataManager = new {LibraryNamespace}.DataManager(top.recordRepository, top.deepInsertService, top.recordService, [new {LibraryNamespace}.FakerPreprocessor()]);");
            }

            scriptBuilder.AppendLine($"{TestDriverReference} = new {LibraryNamespace}.Driver(top.dataManager);");

            this.javascriptExecutor.ExecuteScript(scriptBuilder.ToString());
        }

        /// <inheritdoc cref="ITestDriver"/>
        public void LoadTestData(string data)
        {
            this.ExecuteDriverFunctionAsync($"loadTestData(`{data}`)");
        }

        /// <inheritdoc/>
        public void LoadTestDataAsUser(string data, string username)
        {
            this.ExecuteDriverFunctionAsync($"loadTestDataAsUser(`{data}`, '{username}')");
        }

        /// <inheritdoc cref="ITestDriver"/>
        public void DeleteTestData()
        {
            this.ExecuteDriverFunctionAsync("deleteTestData()");
        }

        /// <inheritdoc cref="ITestDriver"/>
        public void OpenTestRecord(string recordAlias)
        {
            this.ExecuteDriverFunctionAsync($"openTestRecord('{recordAlias}')");
        }

        /// <inheritdoc/>
        public EntityReference GetTestRecordReference(string recordAlias)
        {
            var obj = (Dictionary<string, object>)this.ExecuteDriverFunction($"getRecordReference('{recordAlias}')");

            return new EntityReference((string)obj["entityType"], Guid.Parse((string)obj["id"]));
        }

        private static string GetExecuteScriptForAsyncFunction(string functionCall)
        {
            return $"{TestDriverReference}.{functionCall}.then(arguments[arguments.length - 1]).catch(e => {{ arguments[arguments.length - 1](`{ErrorPrefix}: ${{ e.message }}`); }});";
        }

        private object ExecuteDriverFunction(string functionCall)
        {
            return this.javascriptExecutor.ExecuteScript($"return {TestDriverReference}.{functionCall};");
        }

        private object ExecuteDriverFunctionAsync(string functionCall)
        {
            var result = this.javascriptExecutor.ExecuteAsyncScript(GetExecuteScriptForAsyncFunction(functionCall));

            if (result is string str && str.StartsWith(ErrorPrefix, StringComparison.InvariantCulture))
            {
                throw new WebDriverException(str);
            }

            return result;
        }
    }
}
