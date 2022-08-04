namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using TechTalk.SpecFlow;

    /// <summary>
    /// Steps relating to global search.
    /// </summary>
    [Binding]
    public class GlobalSearchSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Performs an advanced search using the filter attribute and a filter value.
        /// </summary>
        /// <param name="filterBy">Attribute to filter by.</param>
        /// <param name="filterValue">Attribute filter value.</param>
        [When("I apply a search filter using the filter '(.*)' with the filter value of '(.*)' group")]
        public static void WhenIPerformASearchAndFilterByValue(string filterBy, string filterValue)
        {
            XrmApp.GlobalSearch.Filter(filterBy, filterValue);
        }

        /// <summary>
        /// Performs a wildcard search using a particular filter value.
        /// </summary>
        /// <param name="filterValue">Attribute filter value.</param>
        [When("I apply a search filter using the filter '(.*)'")]
        public static void WhenIFilterWithValue(string filterValue)
        {
            XrmApp.GlobalSearch.FilterWith(filterValue);
        }

        /// <summary>
        /// Performs an advanced search using the filter attribute and a filter value.
        /// </summary>
        /// <param name="filterValue">Attribute filter value.</param>
        [When("I search globally using the filter '(.*)'")]
        public static void WhenIPerformASearch(string filterValue)
        {
            XrmApp.GlobalSearch.Search(filterValue);
        }

        /// <summary>
        /// Open a record from a global search at a certain row.
        /// </summary>
        /// <param name="entityName">Attribute to filter by.</param>
        /// <param name="recordIndex">Attribute filter value.</param>
        [When("I open a record from global search on the entity '(.*)' in the position of '(.*)'")]
        public static void WhenIOpenARecordFromSearchUsingIndex(string entityName, int recordIndex)
        {
            XrmApp.GlobalSearch.OpenRecord(entityName, recordIndex);
        }
    }
}
