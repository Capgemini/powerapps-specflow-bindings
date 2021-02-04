namespace Capgemini.PowerApps.SpecFlowBindings.Steps.StepArgument
{
    /// <summary>
    /// HumanReadableIntegerExpression
    /// </summary>
    public class HumanReadableIntegerExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HumanReadableIntegerExpression"/> class.
        /// HumanReadable Integer Expression.
        /// </summary>
        /// <param name="value">numeric value.</param>
        public HumanReadableIntegerExpression(int value) { this.Value = value; }

        /// <summary>
        /// Gets numeric value.
        /// </summary>
        public int Value { get; }
    }
}
