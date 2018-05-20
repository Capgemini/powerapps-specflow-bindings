using System;
using System.Collections.Generic;
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
            return Users.First(user => user.Alias == userAlias);
        }

        /// <summary>
        /// Retrieves the configuration for an app.
        /// </summary>
        /// <param name="appAlias">The alias of the app</param>
        /// <returns></returns>
        public XrmAppConfiguration GetAppConfiguration(string appAlias)
        {
            return Apps.First(app => app.Name == appAlias);
        }
    }
}
