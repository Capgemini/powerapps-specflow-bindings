namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using System;
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
            var user = TestConfig.GetUser(userAlias);

            if (TestConfig.UseProfiles && TestConfig.BrowserOptions.BrowserType.SupportsProfiles())
            {
                TestConfig.BrowserOptions.ProfileDirectory = UserProfileDirectories[user.Username];
                ForgetExistingAccounts(TestConfig.GetTestUrl());
            }

            XrmApp.OnlineLogin.Login(
                TestConfig.GetTestUrl(),
                user.Username.ToSecureString(),
                user.Password.ToSecureString(),
                string.Empty.ToSecureString());

            XrmApp.Navigation.OpenApp(appName);

            CloseTeachingBubbles();
        }

        private static void CloseTeachingBubbles()
        {
            foreach (var closeButton in Driver.FindElements(By.ClassName("ms-TeachingBubble-closebutton")))
            {
                closeButton.Click();
            }
        }

        // This logic is only required as there is currently a defect in Easy Repro which causes it to not handle the "Pick and Account" dialog
        // This can be removed when the PR into EasyRepro is merged. https://github.com/microsoft/EasyRepro/pull/1143
        private static void ForgetExistingAccounts(Uri orgUrl)
        {
            Client.Browser.Driver.Navigate().GoToUrl(orgUrl);

            foreach (var existingAccountMenuButton in Driver.FindElements(By.ClassName("tile-menu")))
            {
                existingAccountMenuButton.Click();
                Driver.FindElement(By.Id("forgetLink")).Click();
            }
        }
    }
}
