using Capgemini.Test.Xrm.Bindings.Core;
using Capgemini.Test.Xrm.Configuration;
using Capgemini.Test.Xrm.EasyRepro.Web.Extensions;
using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
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
        [Given(@"I am logged in as ""(.*)""")]
        public void GivenIAmLoggedInAs(string user)
        {
            Login(XrmTestConfig.GetUserConfiguration(user));
        }

        [Given(@"I am logged in to the ""(.*)"" app as ""(.*)""")]
        public void GivenIAmLoggedInToTheAppAs(string appName, string user)
        {
            Login(XrmTestConfig.GetUserConfiguration(user), XrmTestConfig.GetAppConfiguration(appName).Id);
        }

        [Given(@"I have opened the ""(.*)"" dashboard")]
        public void GivenIHaveOpenedTheDashoard(string dashboardName)
        {
            Browser.Navigation.OpenSubArea("Sales", "Dashboards");
            Browser.Dashboard.SelectDashBoard(dashboardName);
        }

        [Given(@"I have opened the ""(.*)"" sub-area of the ""(.*)"" area")]
        public void GivenIHaveOpenedTheSubAreaOfTheArea(string subAreaName, string areaName)
        {
            Browser.Navigation.OpenSubArea(areaName, subAreaName);
        }

        [Given(@"I have selected the ""(.*)"" command")]
        public void GivenIHaveSlectedToCreateANewRecord(string commandName)
        {
            Browser.CommandBar.ClickCommand(commandName);
        }
        #endregion

        #region When
        #endregion

        #region Then
        #endregion

        private BrowserCommandResult<LoginResult> Login(XrmUserConfiguration userConfig, string appId = "")
        {
            return Browser.LoginPage.Login(
                new Uri($"{XrmTestConfig.Url}/main.aspx?{(appId != string.Empty ? "appId=" : "")}{appId}"),
                userConfig.Username,
                userConfig.Password,
                userConfig.IsAdfs);
        }
    }
}
