namespace Capgemini.PowerApps.SpecFlowBindings
{
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Reads test data from JSON files.
    /// </summary>
    public class FileDataRepository : ITestDataRepository
    {
        private const string FileDirectory = "data";
        private static readonly string RootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <inheritdoc cref="ITestDataRepository"/>
        public string GetTestData(string identifier)
        {
            return File.ReadAllText(Path.Combine(RootDirectory, FileDirectory, Path.GetExtension(identifier) == ".json" ? identifier : $"{identifier}.json"));
        }
    }
}