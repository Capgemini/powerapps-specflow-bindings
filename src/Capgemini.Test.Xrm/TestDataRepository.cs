using System.IO;
using System.Reflection;
using Capgemini.Test.Xrm.Data.Core;

namespace Capgemini.Test.Xrm.Data
{
    /// <summary>
    /// Reads test data from JSON files.
    /// </summary>
    public class TestDataRepository : ITestDataRepository
    {
        private static readonly string RootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private const string FileDirectory = "Data";

        /// <inheritdoc cref="ITestDataRepository"/>
        /// <param name="fileName">The name of the JSON file.</param>
        public string GetTestData(string fileName)
        {
            return File.ReadAllText(Path.Combine(RootDirectory, FileDirectory, Path.HasExtension(fileName) ? fileName : $"{fileName}.json"));
        }
    }
}