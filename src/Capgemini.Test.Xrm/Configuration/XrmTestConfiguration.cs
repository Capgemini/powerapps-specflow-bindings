using System;
using System.Configuration;
using System.Linq;
using YamlDotNet.Serialization;

namespace Capgemini.Test.Xrm.Configuration
{
    /// <summary>
    /// Test configuration for Dynamics 365 automated UI testing.
    /// </summary>
    public class XrmTestConfiguration
    {
        public const string FileName = "spec-xrm.yaml";
        private const string GetUserException = "Unable to retrieve user configuration. Please ensure a user with the given alias exists in the config.";
        private const string GetAppException = "Unable to retrieve app configuration. Please ensure an app with the given name exists in the config.";

        /// <summary>
        /// The URL of the target Dynamics 365 instance.
        /// </summary>
        [YamlMember(Alias = "url")]
        public string Url { get; set; }

        /// <summary>
        /// The URL of the WebDriver remote server.
        /// </summary>
        [YamlMember(Alias = "remoteServerUrl")]
        public string RemoteServerUrl { get; set; }

        /// <summary>
        /// The browser to use for running tests.
        /// </summary>
        [YamlMember(Alias = "browser")]
        public Browser Browser { get; set; }

        /// <summary>
        /// Users that tests can be run as.
        /// </summary>
        [YamlMember(Alias = "users")]
        public XrmUserConfiguration[] Users { get; set; }

        /// <summary>
        /// Apps that tests can navigate to.
        /// </summary>
        [YamlMember(Alias = "apps")]
        public XrmAppConfiguration[] Apps { get; set; }

        /// <summary>
        /// Retrieves the configuration for a user.
        /// </summary>
        /// <param name="userAlias">The alias of the user</param>
        /// <returns></returns>
        public XrmUserConfiguration GetUserConfiguration(string userAlias)
        {
            try
            {
                return Users.First(user => user.Alias == userAlias);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException($"{GetUserException} User: {userAlias}", ex);
            }
        }

        /// <summary>
        /// Retrieves the configuration for an app.
        /// </summary>
        /// <param name="appName">The name of the app</param>
        /// <returns></returns>
        public XrmAppConfiguration GetAppConfiguration(string appName)
        {
            try
            {
                return Apps.First(app => app.Name == appName);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException($"{GetAppException} App: {appName}", ex);
            }
        }
    }
}
