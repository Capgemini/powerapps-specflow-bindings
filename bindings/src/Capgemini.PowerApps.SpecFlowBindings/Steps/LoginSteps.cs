namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using System;
    using System.IO;
    using Capgemini.PowerApps.SpecFlowBindings.Extensions;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using OpenQA.Selenium;
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
            var user = TestConfig.GetUser(userAlias, useCurrentUser: false);

            if (TestConfig.UseProfiles && TestConfig.BrowserOptions.BrowserType.SupportsProfiles())
            {
                SetupScenarioProfile(user.Username);
            }

            var url = TestConfig.GetTestUrl();

            if (!string.IsNullOrEmpty(user.OtpToken))
            {
                XrmApp.OnlineLogin.Login(url, user.Username.ToSecureString(), user.Password.ToSecureString(), user.OtpToken.ToSecureString());
            }
            else
            {
                XrmApp.OnlineLogin.Login(url, user.Username.ToSecureString(), user.Password.ToSecureString());
            }

            if (!url.Query.Contains("appid"))
            {
                XrmApp.Navigation.OpenApp(appName);
            }

            Driver.WaitForTransaction();

            CloseTeachingBubbles();
        }

        private static void CloseTeachingBubbles()
        {
            foreach (var closeButton in Driver.FindElements(By.ClassName("ms-TeachingBubble-closebutton")))
            {
                closeButton.Click();
            }
        }

        private static bool WaitForMainPage(IWebDriver driver, TimeSpan? timeout = null)
        {
            timeout = timeout ?? 10.Seconds();

            var isUCI = driver.HasElement(By.XPath(Elements.Xpath[Reference.Login.CrmUCIMainPage]));
            if (isUCI)
            {
                driver.WaitForTransaction();
            }

            var xpathToMainPage = By.XPath(Elements.Xpath[Reference.Login.CrmMainPage]);
            var element = driver.WaitUntilAvailable(xpathToMainPage, timeout);

            return element != null;
        }

        private static void SetupScenarioProfile(string username)
        {
            var baseProfileDirectory = Path.Combine(UserProfileDirectories[username], "base");
            new DirectoryInfo(baseProfileDirectory).CopyTo(new DirectoryInfo(CurrentProfileDirectory));
        }
    }
}
