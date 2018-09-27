using Capgemini.Test.Xrm.Web.Core;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Web.Steps.Given
{
    [Binding]
    public class GivenSteps : XrmWebStepDefiner
    {
        [Given(@"I have opened (.*)")]
        public void GivenIHaveOpened(string alias)
        {
            TestDriver.OpenTestRecord(alias);
        }

        [Given(@"I have created (.*)")]
        public void GivenIHaveCreated(string fileName)
        {
            TestDriver.LoadTestData(TestDataRepository.GetTestData(fileName));
        }
    }
}
