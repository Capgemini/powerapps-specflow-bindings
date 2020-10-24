namespace Capgemini.PowerApps.SpecFlowBindings.Extensions
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Capgemini.PowerApps.SpecFlowBindings.Transformations.Tokenisation;

    /// <summary>
    /// Extensions to the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Looks for templated text in string and replaces.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The resulting string.</returns>
        public static string ReplaceTemplatedText(this string input)
        {
            input = input ?? throw new ArgumentNullException(nameof(input));

            MatchCollection matches = Regex.Matches(input, "{{.*}}");
            var random = new Random();

            foreach (Match match in matches)
            {
                string innerText = match.Value.Substring(2, match.Value.Length - 4);
                string[] templateSplit = innerText.Split(':');

                if (templateSplit[0].ToUpperInvariant() == "RANDNUM")
                {
                    string[] randomBound = templateSplit[1].Split(',');
                    int lowerBound = int.Parse(randomBound[0], CultureInfo.CurrentCulture);
                    int upperBound = int.Parse(randomBound[1], CultureInfo.CurrentCulture);

                    input = input
                        .Remove(
                            match.Index,
                            match.Length)
                        .Insert(
                            match.Index,
                            random
                                .Next(lowerBound, upperBound)
                                .ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    throw new NotImplementedException($"ReplaceTemplatedText does not support {templateSplit[0]}.");
                }
            }

            return input;
        }

        /// <summary>
        /// Replaces all tokens in the json file with the available functions.
        /// </summary>
        /// <param name="input">String.</param>
        /// <returns>String with the tokenised output.</returns>
        public static string ReplaceTemplatedTokens(this string input)
        {
            TokenFactory.GetHandlers().ForEach(handler =>
            {
                input = handler.ReplaceTokens(input);
            });

            return input;
        }
    }
}
