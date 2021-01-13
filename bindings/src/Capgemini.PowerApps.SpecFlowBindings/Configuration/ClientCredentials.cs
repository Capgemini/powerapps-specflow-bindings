namespace Capgemini.PowerApps.SpecFlowBindings.Configuration
{
    using YamlDotNet.Serialization;

    /// <summary>
    /// Configuration for the test application user.
    /// </summary>
    public class ClientCredentials
    {
        private string tenantId;
        private string clientId;
        private string clientSecret;

        /// <summary>
        /// Gets or sets the tenant ID of the test application user app registration.
        /// </summary>
        [YamlMember(Alias = "tenantId")]
        public string TenantId { get => ConfigHelper.GetEnvironmentVariableIfExists(this.tenantId); set => this.tenantId = value; }

        /// <summary>
        /// Gets or sets the client ID of the test application user app registration.
        /// </summary>
        [YamlMember(Alias = "clientId")]
        public string ClientId { get => ConfigHelper.GetEnvironmentVariableIfExists(this.clientId); set => this.clientId = value; }

        /// <summary>
        /// Gets or sets a client secret or the name of an environment variable containing the client secret of the test application user app registration.
        /// </summary>
        [YamlMember(Alias = "clientSecret")]
        public string ClientSecret { get => ConfigHelper.GetEnvironmentVariableIfExists(this.clientSecret); set => this.clientSecret = value; }
    }
}
