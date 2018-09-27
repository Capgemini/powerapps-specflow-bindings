using Capgemini.Test.Xrm.Configuration;
using Capgemini.Test.Xrm.Web.Core;
using Capgemini.Test.Xrm.Web.Extensions;
using System;
using TechTalk.SpecFlow;

namespace Capgemini.Test.Xrm.Web.Steps.Given
{
    [Binding]
    public class LoginSteps : XrmWebStepDefiner
    {
        [Given(@"I am logged in as (.*)")]
        public void GivenIAmLoggedInAs(string user)
        {
            Login(XrmTestConfig.GetUserConfiguration(user));
        }

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
