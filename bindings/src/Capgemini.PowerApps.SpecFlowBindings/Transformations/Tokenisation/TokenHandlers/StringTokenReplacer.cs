namespace Capgemini.PowerApps.SpecFlowBindings.Transformations.Tokenisation.TokenHandlers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Handles string replacement.
    /// </summary>
    public class StringTokenReplacer : BaseTokenHandler
    {
        /// <summary>
        /// Gets the regex.
        /// </summary>
        /// <returns>Returns the Regex.</returns>
        public override Regex GetRegex()
        {
            return new Regex(@"#{RandomString\[\d+]}#");
        }

        /// <summary>
        /// Does the class support lengths.
        /// </summary>
        /// <returns>Boolean.</returns>
        public override bool MatchContainsNumber()
        {
            return true;
        }

        /// <summary>
        /// Does the class support lengths.
        /// </summary>
        /// <returns>Boolean.</returns>
        public override string Generate(Match m)
        {
            var allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (this.MatchContainsNumber())
            {
                Random random = new Random();
                var amount = new Regex(@"\d+").Match(m.Value.ToString());
                var randomString = new string(Enumerable.Repeat(allowedChars, Convert.ToInt32(amount.Value, CultureInfo.CurrentCulture))
                            .Select(s => s[random.Next(s.Length)]).ToArray());

                return randomString;
            }

            return string.Empty;
        }
    }
}
