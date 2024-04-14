namespace Capgemini.PowerApps.SpecFlowBindings.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Capgemini.PowerApps.SpecFlowBindings.Extensions;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Edge;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;

    /// <summary>
    /// Extends the EasyRepro <see cref="BrowserOptions"/> class with support for additional configuration.
    /// </summary>
    public class BrowserOptionsWithProfileSupport : BrowserOptions, ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserOptionsWithProfileSupport"/> class.
        /// </summary>
        public BrowserOptionsWithProfileSupport()
            : base()
        {
            this.CookieСontrolsMode = 0;
            this.AdditionalCapabilities = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the directory to use as the user profile.
        /// </summary>
        public string ProfileDirectory { get; set; }

        /// <summary>
        /// Gets or sets the additional capabilities.
        /// </summary>
        public Dictionary<string, object> AdditionalCapabilities { get; set; }

        /// <inheritdoc/>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <inheritdoc/>
        public override ChromeOptions ToChrome()
        {
            var options = base.ToChrome();

            if (!string.IsNullOrEmpty(this.ProfileDirectory))
            {
                options.AddArgument($"--user-data-dir={this.ProfileDirectory}");
            }

            this.AddAdditionalCapabilities(options);

            return options;
        }

        /// <inheritdoc/>
        public override FirefoxOptions ToFireFox()
        {
            var options = base.ToFireFox();

            if (!string.IsNullOrEmpty(this.ProfileDirectory))
            {
                this.ProfileDirectory = this.ProfileDirectory.EndsWith("firefox") ? this.ProfileDirectory : Path.Combine(this.ProfileDirectory, "firefox");
                options.AddArgument($"-profile \"{this.ProfileDirectory}\"");
            }

            this.AddAdditionalCapabilities(options);

            return options;
        }

        /// <inheritdoc/>
        public override EdgeOptions ToEdge()
        {
            var options = base.ToEdge();

            this.AddAdditionalCapabilities(options);

            return options;
        }

        /// <inheritdoc/>
        public override InternetExplorerOptions ToInternetExplorer()
        {
            var options = base.ToInternetExplorer();

            this.AddAdditionalCapabilities(options);

            return options;
        }

        private void AddAdditionalCapabilities(DriverOptions options)
        {
            foreach (var desiredCapability in this.AdditionalCapabilities)
            {
                options.AddGlobalCapability(desiredCapability.Key, desiredCapability.Value);
            }
        }
    }
}
