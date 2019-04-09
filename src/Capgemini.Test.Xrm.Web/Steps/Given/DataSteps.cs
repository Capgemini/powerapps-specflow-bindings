using Capgemini.Test.Xrm.Web.Core;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Web.Steps.Given
{
    /// <summary>
    /// Test setup step bindings for data.
    /// </summary>
    [Binding]
    public class GivenSteps : XrmWebStepDefiner
    {
        /// <summary>
        /// Opens a test record.
        /// </summary>
        /// <param name="alias">The alias of the test record.</param>
        [Given(@"I have opened (.*)")]
        public void GivenIHaveOpened(string alias)
        {
            TestDriver.OpenTestRecord(alias);
        }

        /// <summary>
        /// Creates a test record.
        /// </summary>
        /// <param name="fileName">The name of the file containing the test record.</param>
        [Given(@"I have created (.*)")]
        public void GivenIHaveCreated(string fileName)
        {
            TestDriver.LoadTestData(TestDataRepository.GetTestData(fileName));
        }
    }
}
