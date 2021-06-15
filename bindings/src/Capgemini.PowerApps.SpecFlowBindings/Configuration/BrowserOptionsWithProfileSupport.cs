namespace Capgemini.PowerApps.SpecFlowBindings.Configuration
{
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using OpenQA.Selenium.Chrome;

    /// <summary>
    /// Extends the EasyRepro <see cref="BrowserOptions"/> class with additonal support for chrome profiles.
    /// </summary>
    public class BrowserOptionsWithProfileSupport : BrowserOptions
    {
        /// <summary>
        /// Gets or sets the directory to use as the user profile.
        /// </summary>
        public string ChromeProfileDirectory { get; set; }

        /// <inheritdoc/>
        public override ChromeOptions ToChrome()
        {
            var options = base.ToChrome();
            if (!string.IsNullOrEmpty(this.ChromeProfileDirectory))
            {
                options.AddArgument($"--user-data-dir={this.ChromeProfileDirectory}");
            }

            return options;
        }
    }
}
