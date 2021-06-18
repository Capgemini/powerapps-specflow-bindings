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

        private object userEnumeratorsLock = new object();
        private Dictionary<string, IEnumerator<UserConfiguration>> userEnumerators;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestConfiguration"/> class.
        /// </summary>
        public TestConfiguration()
        {
            this.BrowserOptions = new BrowserOptions();
        }

        /// <summary>
        /// Sets the URL of the target Dynamics 365 instance.
        /// </summary>
        [YamlMember(Alias = "url")]
        public string Url { private get; set; }

        /// <summary>
        /// Gets or sets the browser options to use for running tests.
        /// </summary>
        [YamlMember(Alias = "browserOptions")]
        public BrowserOptions BrowserOptions { get; set; }

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

        [YamlIgnore]
        private Dictionary<string, IEnumerator<UserConfiguration>> UserEnumerators
        {
            get
            {
                lock (this.userEnumeratorsLock)
                {
                    if (this.userEnumerators == null)
                    {
                        this.userEnumerators = this.Users
                            .Select(user => user.Alias)
                            .Distinct()
                            .ToDictionary(
                                alias => alias,
                                alias => this.Users.Where(u => u.Alias == alias).GetEnumerator());
                    }
                }

                return this.userEnumerators;
            }
        }

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
            UserConfiguration user;

            try
            {
                lock (this.userEnumeratorsLock)
                {
                    var aliasEnumerator = this.UserEnumerators[userAlias];
                    if (!aliasEnumerator.MoveNext())
                    {
                        this.UserEnumerators[userAlias] = this.Users.Where(u => u.Alias == userAlias).GetEnumerator();
                        aliasEnumerator = this.UserEnumerators[userAlias];
                        aliasEnumerator.MoveNext();
                    }

                    user = aliasEnumerator.Current;
                }
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException($"{GetUserException} User: {userAlias}", ex);
            }

            return user;
        }
    }
}