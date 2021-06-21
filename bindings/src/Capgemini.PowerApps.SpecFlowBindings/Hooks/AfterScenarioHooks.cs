namespace Capgemini.PowerApps.SpecFlowBindings.Hooks
{
    using Capgemini.PowerApps.SpecFlowBindings.Configuration;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using OpenQA.Selenium;
    using Polly;
    using System;
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
                Console.WriteLine(screenshotsFolder);

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
        [AfterScenario(Order = 0)]
        public void CleanUpProfileDirectory()
        {
            Client.Browser.BrowserDisposing += new EventHandler<EventArgs>(this.TryCleanupProfile);
        }

        private void TryCleanupProfile(object sender, EventArgs e)
        {
            BrowserOptionsWithProfileSupport options = (sender as InteractiveBrowser).Options as BrowserOptionsWithProfileSupport;
            var retryPolicy = Policy
                .Handle<UnauthorizedAccessException>()
                .Or<IOException>()
                .WaitAndRetry(new[]
                {
                        3.Seconds(),
                        5.Seconds(),
                        10.Seconds(),
                        15.Seconds(),
                });
            var fallbackPolicy = Policy
                .Handle<UnauthorizedAccessException>()
                .Or<IOException>()
                .Fallback(() =>
                {
                    // Give up trying to delete the profile folder.
                    Console.WriteLine("Failed to clean up user data dir as the browser has not exited after 33 seconds.");
                });
            var retryWithFallback = fallbackPolicy.Wrap(retryPolicy);
            retryWithFallback.Execute(() => new DirectoryInfo(options.ProfileDirectory).Delete(true));
        }
    }
}
