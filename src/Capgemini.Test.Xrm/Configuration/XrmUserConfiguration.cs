using YamlDotNet.Serialization;

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
        [YamlMember(Alias = "username")]
        public string Username { get; set; }

        /// <summary>
        /// The password of the user.
        /// </summary>
        [YamlMember(Alias = "password")]
        public string Password { get; set; }

        /// <summary>
        /// Whether or not the user must login via ADFS.
        /// </summary>
        /// 
        [YamlMember(Alias = "isAdfs")]
        public bool IsAdfs { get; set; }

        /// <summary>
        /// The alias of the user (used to retrieve from configuration).
        /// </summary>
        [YamlMember(Alias = "alias")]
        public string Alias { get; set; }
    }
}
