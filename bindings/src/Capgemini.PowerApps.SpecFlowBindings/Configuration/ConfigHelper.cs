namespace Capgemini.PowerApps.SpecFlowBindings.Configuration
{
    using System;

    /// <summary>
    /// Helper methods for configuration classes.
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// Returns the value of an environment variable if it exists. Alternatively, returns the passed in value.
        /// </summary>
        /// <param name="value">The value which may be the name of an environment variable.</param>
        /// <returns>The environment variable value (if found) or the passed in value.</returns>
        public static string GetEnvironmentVariableIfExists(string value)
        {
            var environmentVariableValue = Environment.GetEnvironmentVariable(value);

            if (!string.IsNullOrEmpty(environmentVariableValue))
            {
                return environmentVariableValue;
            }

            return value;
        }
    }
}
