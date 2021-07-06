namespace Capgemini.PowerApps.SpecFlowBindings.Extensions
{
    using System.IO;

    /// <summary>
    /// Extensions to the <see cref="DirectoryInfo"/> class.
    /// </summary>
    public static class DirectoryInfoExtensions
    {
        /// <summary>
        /// Copies the directory recursively to the target directory.
        /// </summary>
        /// <param name="source">The source directory.</param>
        /// <param name="target">The target directory.</param>
        public static void CopyTo(this DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            foreach (var fileInfo in source.GetFiles())
            {
                fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);
            }

            foreach (var subdirectory in source.GetDirectories())
            {
                subdirectory.CopyTo(target.CreateSubdirectory(subdirectory.Name));
            }
        }
    }
}
