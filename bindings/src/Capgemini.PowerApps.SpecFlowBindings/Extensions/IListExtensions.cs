namespace Capgemini.PowerApps.SpecFlowBindings.Extensions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extensions for <see cref="IList{T}"/>.
    /// </summary>
    internal static class IListExtensions
    {
        private static readonly Random Rand = new Random();

        /// <summary>
        /// Randomises the order of a list.
        /// </summary>
        /// <typeparam name="T">The type of list items.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>The shuffled list.</returns>
        internal static IList<T> Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;

            while (n > 1)
            {
                n--;
                var k = Rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
