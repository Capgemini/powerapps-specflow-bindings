namespace Capgemini.PowerApps.SpecFlowBindings.Extensions
{
    using Microsoft.Dynamics365.UIAutomation.Browser;

    /// <summary>
    /// Provides extension methods on <see cref="BrowserType"/>.
    /// </summary>
    public static class BrowserTypeExtensions
    {
        /// <summary>
        /// Determines if the given browser type supports profiles.
        /// </summary>
        /// <param name="type">The <see cref="BrowserType"/> to check.</param>
        /// <returns>true if the browser supports profiles otherwise false.</returns>
        public static bool SupportsProfiles(this BrowserType type)
        {
            switch (type)
            {
                case BrowserType.IE:
                    return false;
                case BrowserType.Chrome:
                    return true;
                case BrowserType.Firefox:
                    return true;
                case BrowserType.Edge:
                    return false;
                case BrowserType.Remote:
                    return false;
                default:
                    return false;
            }
        }
    }
}
