namespace Capgemini.PowerApps.SpecFlowBindings
{
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Reads test data from JSON files.
    /// </summary>
    public class TestDataRepository : ITestDataRepository
    {
        private const string FileDirectory = "data";
        private static readonly string RootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <inheritdoc cref="ITestDataRepository"/>
        /// <param name="fileName">The name of the JSON file.</param>
        public string GetTestData(string fileName)
        {
            return File.ReadAllText(Path.Combine(RootDirectory, FileDirectory, Path.HasExtension(fileName) ? fileName : $"{fileName}.json"));
        }
    }
}