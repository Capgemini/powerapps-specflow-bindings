namespace Capgemini.PowerApps.SpecFlowBindings.MSBuild
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Extend JSON data files using the "@extends" property.
    /// </summary>
    public class ExtendDataFiles : Task
    {
        private const string ExtendsProperty = "@extends";

        private JsonMergeSettings jsonMergeSettings;

        /// <summary>
        /// Gets or sets the items to be extended.
        /// </summary>
        /// <value>
        /// The items to be extended.
        /// </value>
        [Required]
        public ITaskItem[] Include { get; set; }

        /// <summary>
        /// Gets or sets the path to output compiled data files to.
        /// </summary>
        /// <value>
        /// The path to output compiled data files to.
        /// </value>
        [Required]
        public ITaskItem DestinationFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how to handle the merging of arrays <see cref="MergeArrayHandling"/>.
        /// </summary>
        public string ArrayHandling { get; set; }

        /// <summary>
        /// Gets the parsed <see cref="MergeArrayHandling"/> value to use.
        /// </summary>
        /// <value>
        /// The parsed <see cref="MergeArrayHandling"/> value to use.
        /// </value>
        protected MergeArrayHandling MergeArrayHandling => !string.IsNullOrEmpty(this.ArrayHandling) ? (MergeArrayHandling)Enum.Parse(typeof(MergeArrayHandling), this.ArrayHandling) : MergeArrayHandling.Merge;

        private JsonMergeSettings JsonMergeSettings
        {
            get
            {
                if (this.jsonMergeSettings == null)
                {
                    this.jsonMergeSettings = new JsonMergeSettings
                    {
                        MergeArrayHandling = this.MergeArrayHandling,
                        MergeNullValueHandling = MergeNullValueHandling.Merge,
                        PropertyNameComparison = StringComparison.InvariantCultureIgnoreCase,
                    };
                }

                return this.jsonMergeSettings;
            }
        }

        /// <inheritdoc/>
        public override bool Execute()
        {
            var succeeded = true;

            Directory.CreateDirectory(this.DestinationFolder.ItemSpec);

            foreach (var taskItem in this.Include)
            {
                this.CompileDataFile(taskItem.ItemSpec);
            }

            return succeeded;
        }

        private void CompileDataFile(string itemPath)
        {
            this.Log.LogMessage(MessageImportance.Normal, $"Processing data file at '{itemPath}'.");

            var mergeStack = this.GetMergeStack(itemPath);
            var rootJson = mergeStack.Pop();
            while (mergeStack.Count > 0)
            {
                rootJson.Merge(mergeStack.Pop(), this.JsonMergeSettings);
            }

            rootJson.Property(ExtendsProperty)?.Remove();

            File.WriteAllText(this.GetFileOutputPath(itemPath), rootJson.ToString());
        }

        private string GetFileOutputPath(string itemPath)
        {
            return Path.Combine(this.DestinationFolder.ItemSpec, string.Join(" ", itemPath.Split('\\').Skip(1)));
        }

        private Stack<JObject> GetMergeStack(string itemPath, Stack<JObject> existingStack = null)
        {
            var stack = existingStack ?? new Stack<JObject>();
            var data = JObject.Parse(File.ReadAllText(itemPath));
            stack.Push(data);

            var extends = data[ExtendsProperty]?.ToString();
            if (!string.IsNullOrEmpty(extends))
            {
                this.Log.LogMessage(MessageImportance.Low, $"Adding {extends} to merge stack.");
                this.GetMergeStack(
                    Path.Combine(Path.GetDirectoryName(itemPath), $"{extends}.json"),
                    stack);
            }

            return stack;
        }
    }
}
