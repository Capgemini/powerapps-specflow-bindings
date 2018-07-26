using System.IO;
using System.Reflection;
using Capgemini.Test.Xrm.Bindings.Web.Core;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Bindings.Web.Definitions
{
    /// <summary>
    /// Step definitions for scenario setup. These steps should be executed primarily via the Dynamics 365 API.
    /// </summary>
    [Binding]
    public class SetupSteps : XrmWebStepDefiner
    {
        [Given(@"I am viewing the ""(.*)"" record")]
        public void GivenIAmViewingTheRecord(string recordAlias)
        {
            Utility.OpenTestRecord(recordAlias);
        }

        [Given(@"I have created ""(.*)""")]
        public void GivenIHaveCreated(string filePath)
        {
            Utility.LoadTestData(TestDataRepository.GetTestData(filePath));
        }
    }
}
