namespace Capgemini.PowerApps.SpecFlowBindings.Configuration
{
    using YamlDotNet.Serialization;

    /// <summary>
    /// Describes a Dynamics 365 App.
    /// </summary>
    public class XrmAppConfiguration
    {
        /// <summary>
        /// Gets or sets the unique ID of the app.
        /// </summary>
        [YamlMember(Alias = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the app (used to retrieve from configuration).
        /// </summary>
        [YamlMember(Alias = "name")]
        public string Name { get; set; }
    }
}
