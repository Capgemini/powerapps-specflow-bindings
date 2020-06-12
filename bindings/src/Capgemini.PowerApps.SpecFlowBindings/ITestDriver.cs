namespace Capgemini.PowerApps.SpecFlowBindings
{
    /// <summary>
    /// An interface for a test driver used to perform setup and teardown via the Power Apps Client APIs.
    /// </summary>
    public interface ITestDriver
    {
        /// <summary>
        /// Loads scenario test data.
        /// </summary>
        /// <param name="data">The data to load.</param>
        void LoadTestData(string data);

        /// <summary>
        /// Deletes scenario test data.
        /// </summary>
        void DeleteTestData();

        /// <summary>
        /// Open a test record.
        /// </summary>
        /// <param name="recordAlias">The alias of the record.</param>
        void OpenTestRecord(string recordAlias);
    }
}
