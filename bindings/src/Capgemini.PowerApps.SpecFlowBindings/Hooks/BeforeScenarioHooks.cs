
namespace Capgemini.PowerApps.SpecFlowBindings.Hooks
{
    using Reqnroll;

    /// <summary>
    /// Before scenario hooks.
    /// </summary>
    [Binding]
    public class BeforeScenarioHooks : PowerAppsStepDefiner
    {
        /// <summary>
        /// Initialise scenario dependencies.
        /// </summary>
        [BeforeScenario(Order = 0)]
        public void Setup()
        {
            Initialise();
        }
    }
}
