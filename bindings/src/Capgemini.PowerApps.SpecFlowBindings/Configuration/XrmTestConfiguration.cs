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
    public class XrmTestConfiguration
    {
        /// <summary>
        /// The name of the file that stores the test configuration.
        /// </summary>
        public const string FileName = "power-apps-bindings.yml";

        private const string GetUserException = "Unable to retrieve user configuration. Please ensure a user with the given alias exists in the config.";
        private const string GetAppException = "Unable to retrieve app configuration. Please ensure an app with the given name exists in the config.";

        /// <summary>
        /// Gets or sets the URL of the target Dynamics 365 instance.
        /// </summary>
        [YamlMember(Alias = "url")]
        public Uri Url { get; set; }

        /// <summary>
        /// Gets or sets the URL of the WebDriver remote server.
        /// </summary>
        [YamlMember(Alias = "remoteServerUrl")]
        public Uri RemoteServerUrl { get; set; }

        /// <summary>
        /// Gets or sets the browser to use for running tests.
        /// </summary>
        [YamlMember(Alias = "browser")]
        public string Browser { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only

        /// <summary>
        /// Gets or sets users that tests can be run as.
        /// </summary>
        [YamlMember(Alias = "users")]
        public List<XrmUserConfiguration> Users { get; set; }

        /// <summary>
        /// Gets or sets apps that tests can navigate to.
        /// </summary>
        [YamlMember(Alias = "apps")]
        public List<XrmAppConfiguration> Apps { get; set; }

#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// Retrieves the configuration for a user.
        /// </summary>
        /// <param name="userAlias">The alias of the user.</param>
        /// <returns>The user configuration.</returns>
        public XrmUserConfiguration GetUserConfiguration(string userAlias)
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

        /// <summary>
        /// Retrieves the configuration for an app.
        /// </summary>
        /// <param name="appName">The name of the app.</param>
        /// <returns>The app configuration.</returns>
        public XrmAppConfiguration GetAppConfiguration(string appName)
        {
            try
            {
                return this.Apps.First(app => app.Name == appName);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException($"{GetAppException} App: {appName}", ex);
            }
        }
    }
}
