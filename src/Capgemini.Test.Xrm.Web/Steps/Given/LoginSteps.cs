using Capgemini.Test.Xrm.Configuration;
using Capgemini.Test.Xrm.Web.Core;
using Capgemini.Test.Xrm.Web.Extensions;
using System;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Web.Steps.Given
{
    /// <summary>
    /// Test setup step bindings for login.
    /// </summary>
    [Binding]
    public class LoginSteps : XrmWebStepDefiner
    {
        /// <summary>
        /// Logs in to Dynamics 365.
        /// </summary>
        /// <param name="user">The alias of the user to log in as.</param>
        [Given(@"I am logged in as (.*)")]
        public void GivenIAmLoggedInAs(string user)
        {
            Login(XrmTestConfig.GetUserConfiguration(user));
        }

        /// <summary>
        /// Logs in to a Dynamics 365 app.
        /// </summary>
        /// <param name="app">The alias of the app.</param>
        /// <param name="user">The alias of the user.</param>
        [Given(@"I am logged in to the (.*) app as (.*)")]
        public void GivenIAmLoggedInToTheAppAs(string app, string user)
        {
            Login(XrmTestConfig.GetUserConfiguration(user), XrmTestConfig.GetAppConfiguration(app).Id);
        }


        private void Login(XrmUserConfiguration userConfig, string appId = "")
        {
            Browser.LoginPage.Login(
                new Uri($"{XrmTestConfig.Url}/main.aspx?{(appId != string.Empty ? "appId=" : "")}{appId}"),
                userConfig.Username,
                userConfig.Password,
                userConfig.IsAdfs);
        }
    }
}
