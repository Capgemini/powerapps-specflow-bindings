namespace Capgemini.PowerApps.SpecFlowBindings
{
    /// <summary>
    /// Provides an interface for a test data repository.
    /// </summary>
    public interface ITestDataRepository
    {
        /// <summary>
        /// Retrieves the test data for a given identifier.
        /// </summary>
        /// <param name="identifier">The identifier to use.</param>
        /// <returns>A JSON string representing the test data.</returns>
        string GetTestData(string identifier);
    }
}