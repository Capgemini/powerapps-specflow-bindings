namespace Capgemini.PowerApps.SpecFlowBindings.UiTests.Hooks
{
    using System.IO;
    using System.Reflection;
    using Capgemini.PowerApps.SpecFlowBindings;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Reqnroll;

    /// <summary>
    /// After scenario hooks.
    /// </summary>
    [Binding]
    public class AfterScenarioHooks : PowerAppsStepDefiner
    {
        private readonly ScenarioContext scenarioContext;
        private readonly TestContext testContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AfterScenarioHooks"/> class.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        public AfterScenarioHooks(ScenarioContext scenarioContext, TestContext testContext)
        {
            this.scenarioContext = scenarioContext;
            this.testContext = testContext;
        }

        /// <summary>
        /// Publishes the screenshot of the browser when a test fails.
        /// </summary>
        [AfterScenario(Order = 100)]
        public void PublishScreenshotForFailedScenario()
        {
            if (this.scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError)
            {
                var rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var screenshotsFolder = Path.Combine(rootFolder, "screenshots");

                var fileName = string.Concat(this.scenarioContext.ScenarioInfo.Title.Split(Path.GetInvalidFileNameChars()));
                this.testContext.AddResultFile(Path.Combine(screenshotsFolder, $"{fileName}.jpg"));
            }
        }
    }
}
