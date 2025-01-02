namespace Capgemini.PowerApps.SpecFlowBindings.Transformations
{
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using Reqnroll;

    /// <summary>
    /// Transformations for EasyRepro types.
    /// </summary>
    [Binding]
    public static class EasyReproTransformations
    {
        /// <summary>
        /// Transforms a logical name into an option set.
        /// </summary>
        /// <param name="expression">The logical name.</param>
        /// <returns>The option set.</returns>
        [StepArgumentTransformation]
        public static OptionSet TransformFieldNameToOptionSet(string expression)
        {
            return new OptionSet { Name = expression };
        }

        /// <summary>
        /// Transforms a logical name into a multi-value option set.
        /// </summary>
        /// <param name="expression">The logical name.</param>
        /// <returns>The multi-value option set.</returns>
        [StepArgumentTransformation]
        public static MultiValueOptionSet TransformFieldNameToMultiValueOptionSet(string expression)
        {
            return new MultiValueOptionSet { Name = expression };
        }

        /// <summary>
        /// Transforms a logical name into an lookup item.
        /// </summary>
        /// <param name="expression">The logical name.</param>
        /// <returns>The lookup.</returns>
        [StepArgumentTransformation]
        public static LookupItem TransformFieldNameToLookupItem(string expression)
        {
            return new LookupItem { Name = expression };
        }

        /// <summary>
        /// Transforms a logical name into a datetime item.
        /// </summary>
        /// <param name="expression">The logical name.</param>
        /// <returns>The datetime.</returns>
        [StepArgumentTransformation]
        public static DateTimeControl TransformFieldNameToDateTimeItem(string expression)
        {
            return new DateTimeControl(expression);
        }

        /// <summary>
        /// Transforms a logical name into a boolean item.
        /// </summary>
        /// <param name="expression">The logical name.</param>
        /// <returns>The boolean item.</returns>
        [StepArgumentTransformation]
        public static BooleanItem TransformFieldNameToBooleanItem(string expression)
        {
            return new BooleanItem { Name = expression };
        }
    }
}
