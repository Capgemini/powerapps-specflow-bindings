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
        public override string Generate(Match input)
        {
            if (input == null)
            {
                throw new ArgumentNullException($"{nameof(input)} Cannot be null.");
            }

            var allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (this.MatchContainsNumber())
            {
                Random random = new Random();
                var amount = new Regex(@"\d+").Match(input.Value.ToString());
                var randomString = new string(Enumerable.Repeat(allowedChars, Convert.ToInt32(amount.Value, CultureInfo.CurrentCulture))
                            .Select(s => s[random.Next(s.Length)]).ToArray());

                return randomString;
            }

            return string.Empty;
        }
    }
}
