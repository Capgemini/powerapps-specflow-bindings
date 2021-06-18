namespace Capgemini.PowerApps.SpecFlowBindings.Hooks
{
    using System.IO;
    using Capgemini.PowerApps.SpecFlowBindings.Configuration;
    using Capgemini.PowerApps.SpecFlowBindings.Steps;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Hooks that run before the start of each run.
    /// </summary>
    [Binding]
    public class BeforeRunHooks : PowerAppsStepDefiner
    {
        /// <summary>
        /// Creates a new folder for the scenario and copies the session/cookies information from previous runs
        /// </summary>
        [BeforeTestRun]
        public static void BaseProfileSetup()
        {
            if (!TestConfig.UseProfiles)
            {
                return;
            }

            foreach (var username in UserProfileDirectories.Keys)
            {
                var profileDirectory = UserProfileDirectories[username];
                if (Directory.GetDirectories(profileDirectory).Length == 0)
                {
                    var baseDirectory = Path.Combine(profileDirectory, "base");
                    Directory.CreateDirectory(baseDirectory);

                    TestConfig.BrowserOptions.ProfileDirectory = baseDirectory;

                    UserConfiguration user = TestConfig.GetUser(username);
                    LoginSteps.Login(Driver, TestConfig.GetTestUrl(), user.Username, user.Password);
                    Quit();
                }
            }
        }
    }
}
