using Capgemini.Test.Xrm.Bindings.Core;
using Capgemini.Test.Xrm.EasyRepro.Web.Extensions;
using Microsoft.Dynamics365.UIAutomation.Api;
using System;
using System.Configuration;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Bindings.Definitions
{
    /// <summary>
    /// Step definitions for user interactions related to navigation.
    /// </summary>
    [Binding]
    public class NavigationSteps : XrmWebStepDefiner
    {
        #region Given
        [Given(@"I am logged in to the ""(.*)"" app as ""(.*)""")]
        public void GivenIAmLoggedInToTheAppAsA(string appName, string user)
        {
            var userConfig = XrmTestConfig.GetUserConfiguration(user);
            var appConfig = XrmTestConfig.GetAppConfiguration(appName);

            Browser.LoginPage.Login(
                new Uri($"{XrmTestConfig.Url}/main.aspx?appId={appConfig.Id}"),
                userConfig.Username,
                userConfig.Password, userConfig.IsAdfs);
        }
        #endregion

        #region When
        #endregion

        #region Then
        #endregion
    }
}
