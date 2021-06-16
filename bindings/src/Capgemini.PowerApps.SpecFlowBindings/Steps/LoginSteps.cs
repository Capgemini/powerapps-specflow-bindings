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
            }

            Login(Driver, TestConfig.GetTestUrl(), user.Username, user.Password);

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

        private static void Login(IWebDriver driver, Uri orgUrl, string username, string password)
        {
            driver.Navigate().GoToUrl(orgUrl);
            driver.ClickIfVisible(By.Id("otherTile"));

            bool waitForMainPage = WaitForMainPage(driver);

            if (!waitForMainPage)
            {
                IWebElement usernameInput = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Login.UserId]), 30.Seconds());
                usernameInput.SendKeys(username);
                usernameInput.SendKeys(Keys.Enter);

                IWebElement passwordInput = driver.WaitUntilClickable(By.XPath(Elements.Xpath[Reference.Login.LoginPassword]), 30.Seconds());
                passwordInput.SendKeys(password);
                passwordInput.Submit();

                var staySignedIn = driver.WaitUntilClickable(By.XPath(Elements.Xpath[Reference.Login.StaySignedIn]), 10.Seconds());
                if (staySignedIn != null)
                {
                    staySignedIn.Click();
                }

                WaitForMainPage(driver, 30.Seconds());
            }
        }

        private static bool WaitForMainPage(IWebDriver driver, TimeSpan? timeout = null)
        {
            timeout = timeout ?? 10.Seconds();
            bool isUCI = driver.HasElement(By.XPath(Elements.Xpath[Reference.Login.CrmUCIMainPage]));
            if (isUCI)
            {
                driver.WaitForTransaction();
            }

            var xpathToMainPage = By.XPath(Elements.Xpath[Reference.Login.CrmMainPage]);
            var element = driver.WaitUntilAvailable(xpathToMainPage, timeout);
            return element != null;
        }
    }
}
