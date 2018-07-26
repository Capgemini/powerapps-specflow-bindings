using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using System;

namespace Capgemini.Test.Xrm.EasyRepro.Web.Extensions
{
    /// <summary>
    /// Extensions for the LoginDialog.
    /// </summary>
    public static class LoginDialogExtensions
    {
        /// <summary>
        /// Logs in to Dynamics 365 with the given credentials via ADFS if required.
        /// </summary>
        /// <param name="loginDialog">The LoginDialog.</param>
        /// <param name="uri">The URL of the Dynamics 365 instance.</param>
        /// <param name="username">The username to use for login.</param>
        /// <param name="password">The password to use for login.</param>
        /// <param name="isAdfs">True if the uesr must login via ADFS</param>
        /// <returns></returns>
        public static BrowserCommandResult<LoginResult> Login(this LoginDialog loginDialog, Uri uri, string username, string password, bool isAdfs = false)
        {
            return isAdfs
                ? loginDialog.Login(uri, username.ToSecureString(), password.ToSecureString(), AdfsLoginAction)
                : loginDialog.Login(uri, username.ToSecureString(), password.ToSecureString());
        }

        private static void AdfsLoginAction(LoginRedirectEventArgs args)
        {
            args.Driver.FindElement(By.Id("passwordInput")).SendKeys(args.Password.ToUnsecureString());
            args.Driver.ClickWhenAvailable(By.Id("submitButton"), new TimeSpan(0, 0, 2));
            args.Driver.WaitForPageToLoad();

            if (args.Driver.HasElement(By.Id("idSIButton9")))
                args.Driver.ClickWhenAvailable(By.Id("idSIButton9"));
        }
    }
}
