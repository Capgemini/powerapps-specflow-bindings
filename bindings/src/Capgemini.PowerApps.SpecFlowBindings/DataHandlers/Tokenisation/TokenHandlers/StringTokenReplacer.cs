namespace Capgemini.PowerApps.SpecFlowBindings.Transformations.DataHandlers.TokenHandlers
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Handles string replacement.
    /// </summary>
    public class StringTokenReplacer : BaseTokenHandler
    {
        /// <summary>
        /// Specifies the allowed character list.
        /// </summary>
        /// <returns>Returns string.</returns>
        public override string AllowedCharacters()
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        }

        /// <summary>
        /// Determines whether class supports random.
        /// </summary>
        /// <returns>Boolean.</returns>
        public override bool GeneratesRandom()
        {
            return true;
        }

        /// <summary>
        /// Gets the regex.
        /// </summary>
        /// <returns>Returns the Regex.</returns>
        public override Regex GetRegex()
        {
            return new Regex(@"#{RandomString\[\d+]}#");
        }
    }
}
