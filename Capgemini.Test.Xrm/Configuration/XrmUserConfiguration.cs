using System;
using System.Xml.Serialization;

namespace Capgemini.Test.Xrm.Configuration
{
    /// <summary>
    /// A user that tests can run as. 
    /// </summary>
    [Serializable]
    public class XrmUserConfiguration
    {
        /// <summary>
        /// The username of the user.
        /// </summary>
        [XmlElement("Username")]
        public string Username { get; set; }

        /// <summary>
        /// The password of the user.
        /// </summary>
        [XmlElement("Password")]
        public string Password { get; set; }

        /// <summary>
        /// Whether or not the user must login via ADFS.
        /// </summary>
        /// 
        [XmlAttribute("IsAdfs")]
        public bool IsAdfs { get; set; }

        /// <summary>
        /// The alias of the user (used to retrieve from configuration).
        /// </summary>
        [XmlAttribute("Alias")]
        public string Alias { get; set; }
    }
}
