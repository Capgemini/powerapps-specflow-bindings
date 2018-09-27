using YamlDotNet.Serialization;

namespace Capgemini.Test.Xrm.Configuration
{
    /// <summary>
    /// Describes a Dynamics 365 App
    /// </summary>
    public class XrmAppConfiguration
    {
        /// <summary>
        /// The unique ID of the app.
        /// </summary>
        [YamlMember(Alias = "id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the app (used to retrieve from configuration).
        /// </summary>
        [YamlMember(Alias = "name")]
        public string Name { get; set; }
    }
}
