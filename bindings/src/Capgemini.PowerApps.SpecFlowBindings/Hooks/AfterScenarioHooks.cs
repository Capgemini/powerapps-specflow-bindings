namespace Capgemini.PowerApps.SpecFlowBindings.Hooks
{
    using System;
    using System.IO;
    using System.Reflection;
    using OpenQA.Selenium;
    using Reqnroll;

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
                if (TestConfig.DeleteTestData)
                {
                    TestDriver.DeleteTestData();
                }
            }
            catch (WebDriverException)
            {
                // Ignore - tests might have failed before driver was initialised.
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
            if (this.scenarioContext.ScenarioExecutionStatus != ScenarioExecutionStatus.TestError)
            {
                return;
            }

            var rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var screenshotsFolder = Path.Combine(rootFolder, "screenshots");

            if (!Directory.Exists(screenshotsFolder))
            {
                Directory.CreateDirectory(screenshotsFolder);
            }

            var fileName = string.Concat(this.scenarioContext.ScenarioInfo.Title.Split(Path.GetInvalidFileNameChars()));

            try
            {
                Client.Browser.TakeWindowScreenShot(
                    Path.Combine(screenshotsFolder, $"{fileName}.jpg"),
                    ScreenshotImageFormat.Jpeg);
            }
            catch (WebDriverException ex)
            {
                // Don't throw an unhandled exception if the screenshot can't be captured as this will prevent the WebDriver instance from being cleaned up.
                Console.WriteLine($"Failed to capture screenshot: {ex.Message}.");
            }
        }
    }
}
