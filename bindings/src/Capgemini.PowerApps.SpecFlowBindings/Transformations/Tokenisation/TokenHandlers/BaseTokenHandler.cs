namespace Capgemini.PowerApps.SpecFlowBindings.Transformations.Tokenisation.TokenHandlers
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Base Handler for all types
    /// </summary>
    /// <typeparam name="T">T Return type</typeparam>
    public abstract class BaseTokenHandler
    {
        /// <summary>
        /// Gets the Regex for the dedicated Handler.
        /// </summary>
        /// <returns>Regex for the specialised handler.</returns>
        public abstract Regex GetRegex();

        /// <summary>
        /// Flag to state where the Handler accepts integer value, For example StringScramber(10) allows for 10 characters to be passed.
        /// </summary>
        /// <returns>boolean.</returns>
        public abstract bool MatchContainsNumber();

        /// <summary>
        /// Algorthim to generate the random.
        /// </summary>
        /// <param name="m">The regex match</param>
        /// <returns>Returns the Type.</returns>
        public abstract string Generate(Match m);

        /// <summary>
        /// Returns the replaced text.
        /// </summary>
        /// <param name="input">String to tokenise.</param>
        /// <returns>String of tokenised text.</returns>
        public string ReplaceTokens(string input)
        {
            var regex = this.GetRegex();

            MatchCollection matches = regex.Matches(input);
            foreach (Match match in matches)
            {
                var x = this.Generate(match);
                input = input.Replace(match.Value.ToString(), x.ToString());
            }

            return input;
        }
    }
}
