namespace Capgemini.PowerApps.SpecFlowBindings.Configuration
{
    using YamlDotNet.Serialization;

    /// <summary>
    /// A user that tests can run as.
    /// </summary>
    public class UserConfiguration
    {
        private string username;
        private string password;

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        [YamlMember(Alias = "username")]
        public string Username { get => ConfigHelper.GetEnvironmentVariableIfExists(this.username); set => this.username = value; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        [YamlMember(Alias = "password")]
        public string Password { get => ConfigHelper.GetEnvironmentVariableIfExists(this.password); set => this.password = value; }

        /// <summary>
        /// Gets or sets the alias of the user (used to retrieve from configuration).
        /// </summary>
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the OTP token of the user.
        /// </summary>
        [YamlMember(Alias = "otptoken")]
        public string OtpToken { get => ConfigHelper.GetEnvironmentVariableIfExists(this.OtpToken); set => this.OtpToken = value; }
    }
}
