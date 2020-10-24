namespace Capgemini.PowerApps.SpecFlowBindings.Transformations.DataHandlers.TokenHandlers
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Handles string replacement.
    /// </summary>
    public class DateTimeTokenReplacer : BaseTokenHandler
    {
        /// <summary>
        /// Specifies the allowed character list.
        /// </summary>
        /// <returns>Returns string.</returns>
        public override string AllowedCharacters()
        {
            return string.Empty;
        }

        /// <summary>
        /// Generate.
        /// </summary>
        /// <param name="amount">The length of the random.</param>
        /// <returns>Random String.</returns>
        public override string Generate(int amount)
        {
            return DateTime.Now.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Determines if random is supported.
        /// </summary>
        /// <returns>Bool</returns>
        public override bool GeneratesRandom()
        {
            return false;
        }

        /// <summary>
        /// Gets the regex.
        /// </summary>
        /// <returns>Returns the Regex.</returns>
        public override Regex GetRegex()
        {
            return new Regex(@"#{DateTime}#");
        }
    }
}
