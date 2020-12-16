namespace Capgemini.PowerApps.SpecFlowBindings.Transformations
{
    using System;
    using System.Linq;
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
    }
}
