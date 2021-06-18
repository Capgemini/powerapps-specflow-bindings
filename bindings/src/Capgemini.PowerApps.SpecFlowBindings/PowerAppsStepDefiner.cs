﻿namespace Capgemini.PowerApps.SpecFlowBindings
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Capgemini.PowerApps.SpecFlowBindings.Configuration;
    using Capgemini.PowerApps.SpecFlowBindings.Extensions;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using Microsoft.Identity.Client;
    using OpenQA.Selenium;
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
        private static ITestDriver testDriver;

        [ThreadStatic]
        private static ITestDataRepository testDataRepository;

        [ThreadStatic]
        private static WebClient client;

        [ThreadStatic]
        private static XrmApp xrmApp;

        private static Dictionary<string, string> userProfileDirectories;

        /// <summary>
        /// Gets access token used to authenticate as the application user configured for testing.
        /// </summary>
        protected static string AccessToken
        {
            get
            {
                var hostSegments = TestConfig.GetTestUrl().Host.Split('.');

                return GetApp().AcquireTokenForClient(new string[] { $"https://{hostSegments[0]}.api.{hostSegments[1]}.dynamics.com//.default" })
                    .ExecuteAsync()
                    .Result.AccessToken;
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
        protected static WebClient Client => client ?? (client = new WebClient(TestConfig.BrowserOptions));

        /// <summary>
        /// Gets the EasyRepro XrmApp.
        /// </summary>
        protected static XrmApp XrmApp => xrmApp ?? (xrmApp = new XrmApp(Client));

        /// <summary>
        /// Gets the Selenium WebDriver.
        /// </summary>
        protected static IWebDriver Driver => Client.Browser.Driver;

        /// <summary>
        /// Gets provides utilities for test setup/teardown.
        /// </summary>
        protected static ITestDriver TestDriver
        {
            get
            {
                if (testDriver == null)
                {
                    testDriver = new TestDriver((IJavaScriptExecutor)Driver);
                    testDriver.InjectOnPage(TestConfig.ApplicationUser != null ? AccessToken : null);
                }

                return testDriver;
            }
        }

        /// <summary>
        /// Gets the directories for the chrome profiles if the brower type is chrome.
        /// </summary>
        protected static Dictionary<string, string> UserProfileDirectories
        {
            get
            {
                if (!testConfig.BrowserOptions.BrowserType.SupportsProfiles())
                {
                    throw new NotSupportedException($"The {testConfig.BrowserOptions.BrowserType} does not support profiles.");
                }

                if (userProfileDirectories != null)
                {
                    return userProfileDirectories;
                }

                var directoriesDictionary = new Dictionary<string, string>();

                var basePath = string.IsNullOrEmpty(TestConfig.ProfilesBasePath) ? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : TestConfig.ProfilesBasePath;
                string profilesDir = Path.Combine(basePath, "profiles");
                Directory.CreateDirectory(profilesDir);

                foreach (var username in TestConfig.Users.Select(u => u.Username).ToList())
                {
                    if (directoriesDictionary.ContainsKey(username))
                    {
                        continue;
                    }

                    var userProfileDir = Path.Combine(profilesDir, username);
                    Directory.CreateDirectory(userProfileDir);

                    directoriesDictionary.Add(username, userProfileDir);
                }

                userProfileDirectories = directoriesDictionary;
                return userProfileDirectories;
            }
        }

        /// <summary>
        /// Performs any cleanup necessary when quitting the WebBrowser.
        /// </summary>
        protected static void Quit()
        {
            if (xrmApp == null)
            {
                return;
            }

            xrmApp.Dispose();
            xrmApp = null;
            client = null;
            testDriver = null;
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
