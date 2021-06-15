namespace Capgemini.PowerApps.SpecFlowBindings.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
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
            this.BrowserOptions = new BrowserOptionsWithProfileSupport();
        }

        /// <summary>
        /// Sets the URL of the target Dynamics 365 instance.
        /// </summary>
        [YamlMember(Alias = "url")]
        public string Url { private get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use profiles.
        /// </summary>
        [YamlMember(Alias = "useProfiles")]
        public bool UseProfiles { get; set; } = false;

        /// <summary>
        /// Gets or sets the browser options to use for running tests.
        /// </summary>
        [YamlMember(Alias = "browserOptions")]
        public BrowserOptionsWithProfileSupport BrowserOptions { get; set; }

        /// <summary>
        /// Gets or sets users that tests can be run as.
        /// </summary>
        [YamlMember(Alias = "users")]
        public List<UserConfiguration> Users { get; set; }

        /// <summary>
        /// Gets or sets application user client ID and client secret for performing certain operations (e.g. impersonating other users during test data creation).
        /// </summary>
        [YamlMember(Alias = "applicationUser")]
        public ClientCredentials ApplicationUser { get; set; }

        /// <summary>
        /// Gets the target URL.
        /// </summary>
        /// <returns>The URL of the test environment.</returns>
        public Uri GetTestUrl()
        {
            return new Uri(ConfigHelper.GetEnvironmentVariableIfExists(this.Url));
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
                return this.Users.First(u => u.Alias == userAlias);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException($"{GetUserException} User: {userAlias}", ex);
            }
        }
    }
}
