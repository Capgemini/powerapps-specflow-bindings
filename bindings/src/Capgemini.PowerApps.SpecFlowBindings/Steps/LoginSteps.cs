namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Step bindings related to logging in.
    /// </summary>
    [Binding]
    public class LoginSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Logs in to a given app as a given user.
        /// </summary>
        /// <param name="appName">The name of the app.</param>
        /// <param name="userAlias">The alias of the user.</param>
        [Given("I am logged in to the '(.*)' app as '(.*)'")]
        public static void GivenIAmLoggedInToTheAppAs(string appName, string userAlias)
        {
            var user = TestConfig.GetUser(userAlias);

            XrmApp.OnlineLogin.Login(
                TestConfig.GetTestUrl(),
                user.Username.ToSecureString(),
                user.Password.ToSecureString());

            XrmApp.Navigation.OpenApp(appName);
        }
    }
}
