namespace Capgemini.PowerApps.SpecFlowBindings.Transformations
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Capgemini.PowerApps.SpecFlowBindings.Steps.StepArgument;
    using TechTalk.SpecFlow;

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
        /// Transforms HumanReadable Integer Expression to numeric values.
        /// </summary>
        /// <param name="expression">expression value(1st).</param>
        /// <returns>numeric value.</returns>
        [StepArgumentTransformation]
        public static HumanReadableIntegerExpression TransformHumanReadableIntegerExpression(string expression)
        {
            var regEx = new Regex(@"(\d)");

            if (regEx.Match(expression).Success)
            {
                return new HumanReadableIntegerExpression(Convert.ToInt32(regEx.Match(expression).Groups[0].Value) - 1);
            }

            throw new ArgumentException("Unexpected values");
        }
    }
}
