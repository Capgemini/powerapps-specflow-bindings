namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using System;
    using System.IO;
    using System.Web;
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
        /// Logs into the given instance with the given credentials.
        /// </summary>
        /// <param name="driver">WebDriver used to imitate user actions.</param>
        /// <param name="orgUrl">The <see cref="Uri"/> of the instance.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        public static void Login(IWebDriver driver, Uri orgUrl, string username, string password)
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

            var queryParams = $"&flags=easyreproautomation=true,testmode={TestConfig.BrowserOptions.UCITestMode.ToString().ToLower()}&perf={TestConfig.BrowserOptions.UCIPerformanceMode.ToString().ToLower()}";
            var uri = driver.Url;
            if (!uri.Contains(queryParams) && !uri.Contains(HttpUtility.UrlEncode(queryParams)))
            {
                driver.Navigate().GoToUrl($"{uri}{queryParams}");
            }
        }

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
            Login(Driver, url, user.Username, user.Password);

            if (!url.Query.Contains("appid"))
            {
                XrmApp.Navigation.OpenApp(appName);
            }

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
