using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Xrm.Tooling.Connector;
using System.IO;
using System.Reflection;
using TechTalk.SpecFlow;

namespace Capgemini.PowerApps.SpecFlowBindings.UiTests.Hooks
{
    /// <summary>
    /// Temporary hooks related to fixing broken EasyRepro selectors.
    /// </summary>
    [Binding]
    public class EasyReproSelectorFixHooks : PowerAppsStepDefiner
    {
        [BeforeTestRun]
        public static void FixQuickCreateMenuItemSelector()
        {
            AppElements.Xpath[AppReference.Navigation.QuickCreateMenuItems] = "//button[@role='menuitem']";
        }

        [BeforeTestRun]
        public static void FixEntitySubGridOverflowButtonSelector()
        {
            AppElements.Xpath[AppReference.Entity.SubGridOverflowButton] = ".//button[contains(@aria-label, '[NAME]')]";
        }

        [BeforeTestRun]
        public static void FixRelatedCommandBarButtonSelector()
        {
            AppElements.Xpath[AppReference.Related.CommandBarButton] = ".//button[contains(@aria-label, '[NAME]') and contains(@id,'SubGrid')]";
        }
    }
}