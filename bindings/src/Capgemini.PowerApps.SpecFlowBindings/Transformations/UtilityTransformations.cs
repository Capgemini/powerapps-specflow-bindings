namespace Capgemini.PowerApps.SpecFlowBindings.Transformations
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Reqnroll;

    /// <summary>
    /// General utility transformations.
    /// </summary>
    [Binding]
    public static class UtilityTransformations
    {
        /// <summary>
        /// Transforms a comma-separated list to a string array.
        /// </summary>
        /// <param name="expression">The comma-separated list.</param>
        /// <returns>A string array.</returns>
        [StepArgumentTransformation]
        public static string[] TransformCommaSeparatedStringsToArray(string expression)
        {
            return expression?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
        }

        /// <summary>
        /// Transforms an ordinal number (e.g. 1st, 2nd, 3rd etc.) to a zero-based index.
        /// </summary>
        /// <param name="expression">The ordinal number.</param>
        /// <returns>The zero-based index.</returns>
        [StepArgumentTransformation(@"(\d+(?:(?:st)|(?:nd)|(?:rd)|(?:th)))")]
        public static int TransformOrdinalNumberToZeroBasedIndex(string expression)
        {
            return Convert.ToInt32(Regex.Match(expression, @"\d+").Value) - 1;
        }
    }
}
