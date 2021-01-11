namespace Capgemini.PowerApps.SpecFlowBindings
{
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// An interface for a test driver used to perform setup and teardown via the Power Apps Client APIs.
    /// </summary>
    public interface ITestDriver
    {
        /// <summary>
        /// Loads scenario test data.
        /// </summary>
        /// <param name="data">The data to load.</param>
        /// <param name="username">The username of the user to impersonate.</param>
        /// <param name="authToken">Auth token for the impersonating application user.</param>
        void LoadTestDataAsUser(string data, string username, string authToken);

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
        /// Deletes scenario test data using the access token to authenticate.
        /// </summary>
        /// <param name="accessToken">The access token to authenticate with.</param>
        void DeleteTestData(string accessToken);

        /// <summary>
        /// Open a test record.
        /// </summary>
        /// <param name="recordAlias">The alias of the record.</param>
        void OpenTestRecord(string recordAlias);

        /// <summary>
        /// Gets an entity reference to a previously created test record.
        /// </summary>
        /// <param name="recordAlias">The alias of the test record.</param>
        /// <returns>A reference to the created record.</returns>
        EntityReference GetTestRecordReference(string recordAlias);
    }
}
