namespace Capgemini.PowerApps.SpecFlowBindings.Configuration
{
    using YamlDotNet.Serialization;

    /// <summary>
    /// A user that tests can run as.
    /// </summary>
    public class UserConfiguration
    {
        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        [YamlMember(Alias = "username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        [YamlMember(Alias = "password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the alias of the user (used to retrieve from configuration).
        /// </summary>
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }
    }
}
