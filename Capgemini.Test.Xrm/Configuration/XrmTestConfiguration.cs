using System;
using System.Configuration;
using System.Linq;
using System.Xml.Serialization;

namespace Capgemini.Test.Xrm.Configuration
{
    /// <summary>
    /// Test configuration for Dynamics 365 automated UI testing.
    /// </summary>
    [Serializable]
    [XmlRoot("XrmTestConfig")]
    public class XrmTestConfiguration
    {
        private const string GET_USER_EXCEPTION = "Unable to retrieve user configuration. Please ensure a user with the given alias exists in the xrm.test.config file.";
        private const string GET_APP_EXCEPTION = "Unable to retrieve app configuration. Please ensure an app with the given name exists in the xrm.test.config file.";

        /// <summary>
        /// The URL of the target Dynamics 365 instance.
        /// </summary>
        [XmlElement("Url")]
        public string Url { get; set; }

        /// <summary>
        /// Users that tests can be run as.
        /// </summary>
        [XmlArray("Users")]
        [XmlArrayItem("User")]
        public XrmUserConfiguration[] Users { get; set; }

        /// <summary>
        /// Apps that tests can navigate to.
        /// </summary>
        [XmlArray("Apps")]
        [XmlArrayItem("App")]
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
                throw new ConfigurationErrorsException($"{GET_USER_EXCEPTION} User: {userAlias}", ex);
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
                throw new ConfigurationErrorsException($"{GET_APP_EXCEPTION} App: {appName}", ex);
            }
        }
    }
}
