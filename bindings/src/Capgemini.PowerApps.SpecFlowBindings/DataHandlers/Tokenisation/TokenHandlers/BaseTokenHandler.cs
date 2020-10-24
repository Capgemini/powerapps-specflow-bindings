namespace Capgemini.PowerApps.SpecFlowBindings.Transformations.DataHandlers.TokenHandlers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Base Handler for all types.
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
        /// Determines whether class supports random.
        /// </summary>
        /// <returns>Boolean.</returns>
        public abstract bool GeneratesRandom();

        /// <summary>
        /// Specifies the allowed character list.
        /// </summary>
        /// <returns>Returns string.</returns>
        public abstract string AllowedCharacters();

        /// <summary>
        /// Returns the replaced text.
        /// </summary>
        /// <param name="input">String to tokenise.</param>
        /// <returns>String of tokenised text.</returns>
        public string ReplaceTokens(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException($"{nameof(input)} Cannot be null.");
            }

            var regex = this.GetRegex();

            MatchCollection matches = regex.Matches(input);
            foreach (Match match in matches)
            {
                var amount = 0;
                if (this.GeneratesRandom())
                {
                    amount = Convert.ToInt32(new Regex(@"\d+").Match(match.Value.ToString()).Value, CultureInfo.InvariantCulture);
                }

                var generatedValue = this.Generate(amount);
                input = input.Replace(match.Value.ToString(), generatedValue.ToString());
            }

            return input;
        }

        /// <summary>
        /// Generate.
        /// </summary>
        /// <param name="amount">The length of the random.</param>
        /// <returns>Random String.</returns>
        public virtual string Generate(int amount)
        {
            Random r = new Random();
            return new string(Enumerable.Repeat(this.AllowedCharacters(), amount).Select(s => s[r.Next(s.Length)]).ToArray());
        }
    }
}
