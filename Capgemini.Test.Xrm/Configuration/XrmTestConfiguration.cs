using System.Linq;

namespace Capgemini.Test.Xrm.Configuration
{
    /// <summary>
    /// Test configuration for Dynamics 365 automated UI testing.
    /// </summary>
    public class XrmTestConfiguration
    {
        /// <summary>
        /// The URL of the target Dynamics 365 instance.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Users that tests can be run as.
        /// </summary>
        private XrmUserConfiguration[] users { get; set; }

        /// <summary>
        /// Apps that tests can navigate to.
        /// </summary>
        private XrmAppConfiguration[] apps { get; set; }

        /// <summary>
        /// Retrieves the configuration for a user.
        /// </summary>
        /// <param name="userAlias">The alias of the user</param>
        /// <returns></returns>
        public XrmUserConfiguration GetUserConfiguration(string userAlias)
        {
            return users.First(user => user.Alias == userAlias);
        }

        /// <summary>
        /// Retrieves the configuration for an app.
        /// </summary>
        /// <param name="appAlias">The alias of the app</param>
        /// <returns></returns>
        public XrmAppConfiguration GetAppConfiguration(string appAlias)
        {
            return apps.First(app => app.Name == appAlias);
        }
    }
}
