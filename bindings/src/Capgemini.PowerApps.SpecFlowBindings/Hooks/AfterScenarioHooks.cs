namespace Capgemini.PowerApps.SpecFlowBindings.Hooks
{
    using Capgemini.PowerApps.SpecFlowBindings.Extensions;
    using OpenQA.Selenium;
    using System.IO;
    using System.Reflection;
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
            if (this.scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError)
            {
                var rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var screenshotsFolder = Path.Combine(rootFolder, "screenshots");

                if (!Directory.Exists(screenshotsFolder))
                {
                    Directory.CreateDirectory(screenshotsFolder);
                }

                var fileName = string.Concat(this.scenarioContext.ScenarioInfo.Title.Split(Path.GetInvalidFileNameChars()));
                Client.Browser.TakeWindowScreenShot(
                    Path.Combine(screenshotsFolder, $"{fileName}.jpg"),
                    ScreenshotImageFormat.Jpeg);
            }
        }

        /// <summary>
        /// Deletes the user profiles used for this scenario.
        /// </summary>
        [AfterScenario(Order = 2)]
        public void CleanUpProfileDirectory()
        {
            if (this.scenarioContext.ContainsKey("scenarioProfileDir"))
            {
                var scenarioProfileDir = this.scenarioContext["scenarioProfileDir"] as string;
                new DirectoryInfo(scenarioProfileDir).Delete(true);
            }
        }
    }
}
