namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using System;
    using System.Linq;
    using Capgemini.PowerApps.SpecFlowBindings.Extensions;
    using FluentAssertions;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Step bindings related to quick creates.
    /// </summary>
    [Binding]
    public class QuickCreateSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Selects a record by name in a quick create lookup.
        /// </summary>
        /// <param name="name">The name of the record.</param>
        /// <param name="lookup">The lookup logical name.</param>
        [When(@"I select a record named '(.*)' in the '(.*)' lookup on the quick create")]
        public static void WhenISelectARecordNamedInTheLookupOnTheQuickCreate(string name, string lookup)
        {
            XrmApp.QuickCreate.SetValue(new LookupItem { Name = lookup, Value = name });
        }

        /// <summary>
        /// Saves a quick create.
        /// </summary>
        [When(@"I save the quick create")]
        public static void WhenISaveTheQuickCreate()
        {
            XrmApp.QuickCreate.Save();
        }

    }
}
