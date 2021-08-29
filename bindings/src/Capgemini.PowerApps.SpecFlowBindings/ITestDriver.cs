namespace Capgemini.PowerApps.SpecFlowBindings
{
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// An interface for a test driver used to perform setup and teardown via the Power Apps Client APIs.
    /// </summary>
    public interface ITestDriver
    {
        /// <summary>
        /// Injects the driver onto the current page.
        /// </summary>
        /// <param name="authToken">The application user auth token (if configured).</param>
        void InjectOnPage(string authToken);

        /// <summary>
        /// Loads scenario test data.
        /// </summary>
        /// <param name="data">The data to load.</param>
        /// <param name="username">The username of the user to impersonate.</param>
        void LoadTestDataAsUser(string data, string username);

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

        /// <summary>
        /// Open a test record.
        /// </summary>
        /// <param name="formName">The name of the form.</param>
        /// <param name="entityName">The logical name of the entity.</param>
        void OpenForm(string formName, string entityName);

        /// <summary>
        /// Gets an entity reference to a previously created test record.
        /// </summary>
        /// <param name="recordAlias">The alias of the test record.</param>
        /// <returns>A reference to the created record.</returns>
        EntityReference GetTestRecordReference(string recordAlias);
    }
}
