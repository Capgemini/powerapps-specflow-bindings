namespace Capgemini.Test.Xrm.Utilities.Core
{
    public interface ITestUtility
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
        /// <param name="alias"></param>
        void OpenTestRecord(string alias);
    }
}
