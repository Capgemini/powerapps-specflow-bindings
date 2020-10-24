namespace Capgemini.PowerApps.SpecFlowBindings.Transformations.Tokenisation.TokenHandlers
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
        /// Returns the random int.
        /// </summary>
        /// <param name="m">Regex Match from file.</param>
        /// <returns>Int.</returns>
        public override string Generate(Match m)
        {
            return DateTime.Now.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the regex.
        /// </summary>
        /// <returns>Returns the Regex.</returns>
        public override Regex GetRegex()
        {
            return new Regex(@"#{DateTime}#");
        }

        /// <summary>
        /// Does the class support lengths.
        /// </summary>
        /// <returns>Boolean.</returns>
        public override bool MatchContainsNumber()
        {
            return false;
        }
    }
}
