using Capgemini.Test.Xrm.Web.Core;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Web.Steps.When
{
    [Binding]
    public class NavigationSteps : XrmWebStepDefiner
    {
        [When(@"I open the ""(.*)"" sub-area of the ""(.*)"" area")]
        public void WhenIOpenTheSubAreaOfTheArea(string subArea, string area)
        {
            Browser.Navigation.OpenSubArea(area, subArea);
        }
    }
}
