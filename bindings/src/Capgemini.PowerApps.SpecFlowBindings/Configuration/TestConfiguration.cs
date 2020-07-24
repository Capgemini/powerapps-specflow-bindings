namespace Capgemini.PowerApps.SpecFlowBindings.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Test configuration for Dynamics 365 automated UI testing.
    /// </summary>
    public class TestConfiguration
    {
        /// <summary>
        /// The name of the file that stores the test configuration.
        /// </summary>
        public const string FileName = "power-apps-bindings.yml";

        private const string GetUserException = "Unable to retrieve user configuration. Please ensure a user with the given alias exists in the config.";

        /// <summary>
        /// Initializes a new instance of the <see cref="TestConfiguration"/> class.
        /// </summary>
        public TestConfiguration()
        {
            this.BrowserOptions = new BrowserOptions();
        }

#pragma warning disable CA1056 // Uri properties should not be strings
        /// <summary>
        /// Gets or sets the URL of the target Dynamics 365 instance.
        /// </summary>
        [YamlMember(Alias = "url")]
        public string Url { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        /// <summary>
        /// Gets or sets the browser options to use for running tests.
        /// </summary>
        [YamlMember(Alias = "browserOptions")]
        public BrowserOptions BrowserOptions { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        /// <summary>
        /// Gets or sets users that tests can be run as.
        /// </summary>
        [YamlMember(Alias = "users")]
        public List<UserConfiguration> Users { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// Gets the target URL.
        /// </summary>
        /// <returns>The URL of the test environment.</returns>
        public Uri GetTestUrl()
        {
            var urlEnvironmentVariable = Environment.GetEnvironmentVariable(this.Url);

            return string.IsNullOrEmpty(urlEnvironmentVariable) ? new Uri(this.Url) : new Uri(urlEnvironmentVariable);
        }

        /// <summary>
        /// Retrieves the configuration for a user.
        /// </summary>
        /// <param name="userAlias">The alias of the user.</param>
        /// <returns>The user configuration.</returns>
        public UserConfiguration GetUser(string userAlias)
        {
            try
            {
                var user = this.Users.First(u => u.Alias == userAlias);

                var usernameEnvironmentVariable = Environment.GetEnvironmentVariable(user.Username);
                user.Username = string.IsNullOrEmpty(usernameEnvironmentVariable) ? user.Username : usernameEnvironmentVariable;

                var passwordEnvironmentVariable = Environment.GetEnvironmentVariable(user.Password);
                user.Password = string.IsNullOrEmpty(passwordEnvironmentVariable) ? user.Password : passwordEnvironmentVariable;

                return user;
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException($"{GetUserException} User: {userAlias}", ex);
            }
        }
    }
}
