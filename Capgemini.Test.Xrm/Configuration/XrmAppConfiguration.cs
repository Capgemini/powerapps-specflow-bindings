using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        public string Id { get; set; }

        /// <summary>
        /// The name of the app (used to retrieve from configuration).
        /// </summary>
        [XmlAttribute]
        internal string Name { get; set; }
    }
}
