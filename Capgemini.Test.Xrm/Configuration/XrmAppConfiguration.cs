using System;
using System.Xml.Serialization;

namespace Capgemini.Test.Xrm.Configuration
{
    /// <summary>
    /// Describes a Dynamics 365 App
    /// </summary>
    [Serializable]
    public class XrmAppConfiguration
    {
        /// <summary>
        /// The unique ID of the app.
        /// </summary>
        [XmlElement("Id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the app (used to retrieve from configuration).
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }
    }
}
