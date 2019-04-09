using Capgemini.Test.Xrm.Web.Core;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Web.Steps.When
{
    /// <summary>
    /// Interaction step bindings for navigation.
    /// </summary>
    [Binding]
    public class NavigationSteps : XrmWebStepDefiner
    {
        /// <summary>
        /// Opens a sub-area of the sitemap.
        /// </summary>
        /// <param name="subArea">The name of the subarea.</param>
        /// <param name="area">The name of the area.</param>
        [When(@"I open the ""(.*)"" sub-area of the ""(.*)"" area")]
        public void WhenIOpenTheSubAreaOfTheArea(string subArea, string area)
        {
            Browser.Navigation.OpenSubArea(area, subArea);
        }
    }
}
