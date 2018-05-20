using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Capgemini.Test.Xrm.Configuration
{
    /// <summary>
    /// A user that tests can run as. 
    /// </summary>
    public class XrmUserConfiguration
    {
        /// <summary>
        /// The username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password of the user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Whether or not the user must login via ADFS.
        /// </summary>
        [XmlAttribute]
        public bool IsAdfs { get; set; }

        /// <summary>
        /// The alias of the user (used to retrieve from configuration).
        /// </summary>
        [XmlAttribute]
        internal string Alias { get; set; }
    }
}
