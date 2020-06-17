namespace Capgemini.PowerApps.SpecFlowBindings.Hooks
{
    using System.IO;
    using System.Reflection;
    using OpenQA.Selenium;
    using TechTalk.SpecFlow;

    /// <summary>
    /// After scenario hooks.
    /// </summary>
    [Binding]
    public class AfterScenarioHooks : PowerAppsStepDefiner
    {
        private readonly ScenarioContext scenarioContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AfterScenarioHooks"/> class.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        public AfterScenarioHooks(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        /// <summary>
        /// Deletes the test data created during the test and disposes of the browser.
        /// </summary>
        [AfterScenario(Order = 1)]
        public static void TestCleanup()
        {
            try
            {
                TestDriver.DeleteTestData();
            }
            finally
            {
                Quit();
            }
        }

        /// <summary>
        /// Takes a screenshot of the browser when a test fails.
        /// </summary>
        [AfterScenario(Order = 0)]
        public void ScreenshotFailedScenario()
        {
            if (this.scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError)
            {
                var rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var screenshotsFolder = Path.Combine(rootFolder, "screenshots");

                if (!Directory.Exists(screenshotsFolder))
                {
                    Directory.CreateDirectory(screenshotsFolder);
                }

                Client.Browser.TakeWindowScreenShot(
                    Path.Combine(screenshotsFolder, $"{this.scenarioContext.ScenarioInfo.Title}.jpg"),
                    ScreenshotImageFormat.Jpeg);
            }
        }
    }
}
