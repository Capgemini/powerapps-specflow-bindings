namespace Capgemini.PowerApps.SpecFlowBindings.Hooks
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Capgemini.PowerApps.SpecFlowBindings.Configuration;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Hooks that run before the start of each run.
    /// </summary>
    [Binding]
    public class BeforeRunHooks : PowerAppsStepDefiner
    {
        /// <summary>
        /// Creates a new folder for the scenario and copies the session/cookies information from previous runs.
        /// </summary>
        [BeforeTestRun]
        public static void BaseProfileSetup()
        {
            if (!TestConfig.UseProfiles)
            {
                return;
            }

            Parallel.ForEach(UserProfileDirectories.Keys, (username) =>
            {
                var profileDirectory = UserProfileDirectories[username];
                var baseDirectory = Path.Combine(profileDirectory, "base");

                Directory.CreateDirectory(baseDirectory);

                var userBrowserOptions = (BrowserOptionsWithProfileSupport)TestConfig.BrowserOptions.Clone();
                userBrowserOptions.ProfileDirectory = baseDirectory;

                using (var app = new XrmApp(new WebClient(userBrowserOptions)))
                {
                    var user = TestConfig.Users.First(u => u.Username == username);
                    app.OnlineLogin.Login(TestConfig.GetTestUrl(), user.Username.ToSecureString(), user.Password.ToSecureString());
                }
            });
        }
    }
}