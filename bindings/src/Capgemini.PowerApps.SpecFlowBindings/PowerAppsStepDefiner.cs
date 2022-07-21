namespace Capgemini.PowerApps.SpecFlowBindings
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Capgemini.PowerApps.SpecFlowBindings.Configuration;
    using Capgemini.PowerApps.SpecFlowBindings.Extensions;
    using FluentAssertions.Extensions;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using Microsoft.Identity.Client;
    using OpenQA.Selenium;
    using Polly;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    /// <summary>
    /// Base class for defining step bindings.
    /// </summary>
    public abstract class PowerAppsStepDefiner
    {
        private static TestConfiguration testConfig;

        private static IConfidentialClientApplication app;

        [ThreadStatic]
        private static string currentProfileDirectory;

        [ThreadStatic]
        private static ITestDriver testDriver;

        [ThreadStatic]
        private static ITestDataRepository testDataRepository;

        [ThreadStatic]
        private static WebClient client;

        [ThreadStatic]
        private static XrmApp xrmApp;

        private static AuthenticationResult authenticationResult;

        private static IDictionary<string, string> userProfilesDirectories;
        private static object userProfilesDirectoriesLock = new object();

        /// <summary>
        /// Gets access token used to authenticate as the application user configured for testing.
        /// </summary>
        protected static AuthenticationResult NewAuthResult
        {
            get
            {
                var hostSegments = TestConfig.GetTestUrl().Host.Split('.');

                return GetApp().AcquireTokenForClient(new string[] { $"https://{hostSegments[0]}.api.{hostSegments[1]}.dynamics.com//.default" })
                    .ExecuteAsync()
                    .Result;
            }
        }

        /// <summary>
        /// Gets the configuration for the test project.
        /// </summary>
        protected static TestConfiguration TestConfig
        {
            get
            {
                if (testConfig == null)
                {
                    var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), TestConfiguration.FileName);

                    testConfig = new DeserializerBuilder()
                        .WithTypeConverter(new BrowserCredentialsYamlTypeConverter())
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build()
                        .Deserialize<TestConfiguration>(File.ReadAllText(configPath));

                    testConfig.BrowserOptions.DriversPath = ConfigHelper.GetEnvironmentVariableIfExists(testConfig.BrowserOptions.DriversPath);

                    if (testConfig.BrowserOptions.DriversPath == Path.Combine(Directory.GetCurrentDirectory()))
                    {
                        if (Directory.GetFiles(testConfig.BrowserOptions.DriversPath, "*driver.exe").Length == 0)
                        {
                            testConfig.BrowserOptions.DriversPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        }
                    }
                }

                return testConfig;
            }
        }

        /// <summary>
        /// Gets a repository provides access to test data.
        /// </summary>
        protected static ITestDataRepository TestDataRepository => testDataRepository ?? (testDataRepository = new FileDataRepository());

        /// <summary>
        /// Gets the EasyRepro WebClient.
        /// </summary>
        protected static WebClient Client
        {
            get
            {
                if (client == null)
                {
                    var browserOptions = (BrowserOptionsWithProfileSupport)TestConfig.BrowserOptions.Clone();

                    if (TestConfig.UseProfiles)
                    {
                        browserOptions.ProfileDirectory = CurrentProfileDirectory;
                    }

                    client = new WebClient(browserOptions);
                }

                return client;
            }
        }

        /// <summary>
        /// Gets the EasyRepro XrmApp.
        /// </summary>
        protected static XrmApp XrmApp => xrmApp ?? (xrmApp = new XrmApp(Client));

        /// <summary>
        /// Gets the Selenium WebDriver.
        /// </summary>
        protected static IWebDriver Driver => Client.Browser.Driver;

        /// <summary>
        /// Gets the profile directory for the current scenario.
        /// </summary>
        protected static string CurrentProfileDirectory
        {
            get
            {
                if (!testConfig.BrowserOptions.BrowserType.SupportsProfiles())
                {
                    throw new NotSupportedException($"The {testConfig.BrowserOptions.BrowserType} does not support profiles.");
                }

                if (string.IsNullOrEmpty(currentProfileDirectory))
                {
                    var basePath = string.IsNullOrEmpty(TestConfig.ProfilesBasePath) ? Path.GetTempPath() : TestConfig.ProfilesBasePath;
                    currentProfileDirectory = Path.Combine(basePath, "profiles", Guid.NewGuid().ToString());
                }

                return currentProfileDirectory;
            }
        }

        /// <summary>
        /// Gets provides utilities for test setup/teardown.
        /// </summary>
        protected static ITestDriver TestDriver
        {
            get
            {
                if (testDriver == null) // First Time 
                {
                    authenticationResult = NewAuthResult;
                    testDriver = new TestDriver((IJavaScriptExecutor)Driver);
                    testDriver.InjectOnPage(TestConfig.ApplicationUser != null ? authenticationResult?.AccessToken : null);
                }

                if (!(authenticationResult is null) && DateTime.UtcNow >= authenticationResult.ExpiresOn)
                {
                    authenticationResult = NewAuthResult;
                    // Set it in the JS 
                    testDriver.UpdateAccessToken(authenticationResult.AccessToken);
                }

                return testDriver;
            }
        }

        /// <summary>
        /// Gets the directories for the Chrome or Firefox profiles.
        /// </summary>
        protected static IDictionary<string, string> UserProfileDirectories
        {
            get
            {
                if (!testConfig.BrowserOptions.BrowserType.SupportsProfiles())
                {
                    throw new NotSupportedException($"The {testConfig.BrowserOptions.BrowserType} does not support profiles.");
                }

                lock (userProfilesDirectoriesLock)
                {
                    if (userProfilesDirectories != null)
                    {
                        return userProfilesDirectories;
                    }

                    var basePath = string.IsNullOrEmpty(TestConfig.ProfilesBasePath) ? Path.GetTempPath() : TestConfig.ProfilesBasePath;
                    var profilesDirectory = Path.Combine(basePath, "profiles");

                    Directory.CreateDirectory(profilesDirectory);

                    userProfilesDirectories = TestConfig.Users
                        .Where(u => !string.IsNullOrEmpty(u.Password))
                        .Select(u => u.Username)
                        .Distinct()
                        .ToDictionary(u => u, u => Path.Combine(profilesDirectory, u));

                    foreach (var dir in userProfilesDirectories.Values)
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                return userProfilesDirectories;
            }
        }

        /// <summary>
        /// Performs any cleanup necessary when quitting the WebBrowser.
        /// </summary>
        protected static void Quit()
        {
            // Try to dispose, and catch web driver errors that can occur on disposal. Retry the disposal if these occur. Trap the final exception and continue the disposal process.
            var polly = Policy
                .Handle<WebDriverException>()
                .Retry(3, (ex, i) =>
                {
                    Console.WriteLine(ex.Message);
                })
                .ExecuteAndCapture(() =>
                {
                    xrmApp?.Dispose();

                    // Ensuring that the driver gets disposed. Previously we were left with orphan processes and were unable to clean up profile folders. We cannot rely on xrmApp.Dispose to properly dispose of the web driver.
                    var driver = client?.Browser?.Driver;
                    driver?.Dispose();
                });

            xrmApp = null;
            client = null;
            testDriver = null;
            testConfig?.Flush();

            if (!string.IsNullOrEmpty(currentProfileDirectory) && Directory.Exists(currentProfileDirectory))
            {
                var directoryToDelete = currentProfileDirectory;
                currentProfileDirectory = null;

                // CrashpadMetrics-active.pma file can continue to be locked even after quitting Chrome. Requires retries.
                Policy
                    .Handle<UnauthorizedAccessException>()
                    .WaitAndRetry(3, retry => (retry * 5).Seconds())
                    .ExecuteAndCapture(() =>
                    {
                        Directory.Delete(directoryToDelete, true);
                    });
            }
        }

        /// <summary>
        /// Gets the <see cref="IConfidentialClientApplication"/> used to authenticate as the configured application user.
        /// </summary>
        private static IConfidentialClientApplication GetApp()
        {
            if (TestConfig.ApplicationUser == null)
            {
                throw new ConfigurationErrorsException("An application user has not been configured.");
            }

            if (app == null)
            {
                app = ConfidentialClientApplicationBuilder.Create(TestConfig.ApplicationUser.ClientId)
                    .WithTenantId(TestConfig.ApplicationUser.TenantId)
                    .WithClientSecret(TestConfig.ApplicationUser.ClientSecret)
                    .Build();
            }

            return app;
        }
    }
}
