namespace Capgemini.PowerApps.SpecFlowBindings.Transformations.DataHandlers.TokenHandlers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Handles string replacement.
    /// </summary>
    public class IntegerTokenReplacer : BaseTokenHandler
    {
        /// <summary>
        /// Returns the random int.
        /// </summary>
        /// <param name="m">Regex Match from file.</param>
        /// <returns>Int</returns>
        public override string Generate(Match input)
        {
            if (input == null)
            {
                throw new ArgumentNullException($"{nameof(input)} Cannot be null.");
            }

            var allowedChars = "0123456789";

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

        /// <summary>
        /// Gets the regex.
        /// </summary>
        /// <returns>Returns the Regex.</returns>
        public override Regex GetRegex()
        {
            return new Regex(@"#{RandomNumber\[\d+]}#");
        }

        /// <summary>
        /// Does the class support lengths.
        /// </summary>
        /// <returns>Boolean.</returns>
        public override bool MatchContainsNumber()
        {
            return true;
        }
    }
}
