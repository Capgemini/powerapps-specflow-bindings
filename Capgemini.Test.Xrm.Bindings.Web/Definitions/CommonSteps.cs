using Capgemini.Test.Xrm.Bindings.Web.Core;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Bindings.Web.Definitions
{
    /// <summary>
    /// Step definitions for user interactions that are common throughout Dynamics 365.
    /// </summary>
    [Binding]
    public class CommonSteps : XrmWebStepDefiner
    {
        #region Given
        #endregion

        #region When
        [When(@"I click the ""(.*)"" ribbon button")]
        public void WhenIClickTheRibbonButton(string ribbonButtonName)
        {
            Browser.CommandBar.ClickCommand(ribbonButtonName);
        }
        #endregion

        #region Then
        #endregion
    }
}
