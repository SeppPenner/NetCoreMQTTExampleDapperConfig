using System;
using System.Collections.Generic;
using System.Linq;

namespace Storage.Extensions
{
    /// <summary>
    /// This class contains <see cref="IEnumerable{T}"/> extension methods.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Adds a value to the <see cref="IEnumerable{T}"/> if the value isn't <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type of object to use.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/>.</param>
        /// <param name="range">The range of values to add.</param>
        /// <returns>A new <see cref="IEnumerable{T}"/>.</returns>
        // ReSharper disable once UnusedMember.Global
        public static IEnumerable<T> AddRangeIfNotNull<T>(this IEnumerable<T> enumerable, IEnumerable<T> range)
        {
            if (range == null)
            {
                return enumerable;
            }

            var list = enumerable.ToList();

            foreach (var value in range)
            {
                list.AddIfNotNull(value);
            }
            
            return list;
        }

        /// <summary>
        /// Adds a value to the <see cref="IEnumerable{T}"/> if the value isn't <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type of object to use.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/>.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>A new <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<T> AddIfNotNull<T>(this IEnumerable<T> enumerable, T value)
        {
            if (value == null)
            {
                return enumerable;
            }

            var list = enumerable.ToList();
            list.Add(value);
            return list;
        }

        /// <summary>
        /// Splits an <see cref="IEnumerable{T}"/> at n elements and returns all sub <see cref="IEnumerable{T}"/>s.
        /// </summary>
        /// <typeparam name="T">The type of object to use.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to use.</param>
        /// <param name="size">The size at which the <see cref="IEnumerable{T}"/> should be split.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<IEnumerable<T>> SplitList<T>(this IEnumerable<T> enumerable, int size = 30)
        {
            var list = enumerable.ToList();

            for (var i = 0; i < list.Count; i += size)
            {
                yield return list.GetRange(i, Math.Min(size, list.Count - i));
            }
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> with indexes.
        /// </summary>
        /// <typeparam name="T">The type of object to use.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to use.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with indexes.</returns>
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> enumerable)
        {
            return enumerable?.Select((item, index) => (item, index)) ?? new List<(T, int)>();
        }

        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the default equality comparer for the projected type.
        /// </summary>
        /// <remarks>
        /// This operator uses deferred execution and streams the results, although
        /// a set of already-seen keys is retained. If a key is seen multiple times,
        /// only the first element with that key is returned.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="keySelector">Projection for determining "distinctness"</param>
        /// <returns>A sequence consisting of distinct elements from the source sequence,
        /// comparing them by the specified key projection.</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }

        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the specified comparer for the projected type.
        /// </summary>
        /// <remarks>
        /// This operator uses deferred execution and streams the results, although
        /// a set of already-seen keys is retained. If a key is seen multiple times,
        /// only the first element with that key is returned.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="keySelector">Projection for determining "distinctness"</param>
        /// <param name="comparer">The equality comparer to use to determine whether or not keys are equal.
        /// If null, the default equality comparer for <c>TSource</c> is used.</param>
        /// <returns>A sequence consisting of distinct elements from the source sequence,
        /// comparing them by the specified key projection.</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            return source.DistinctByLocal(keySelector, comparer);
        }

        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the specified comparer for the projected type.
        /// </summary>
        /// <remarks>
        /// This operator uses deferred execution and streams the results, although
        /// a set of already-seen keys is retained. If a key is seen multiple times,
        /// only the first element with that key is returned.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="keySelector">Projection for determining "distinctness"</param>
        /// <param name="comparer">The equality comparer to use to determine whether or not keys are equal.
        /// If null, the default equality comparer for <c>TSource</c> is used.</param>
        /// <returns>A sequence consisting of distinct elements from the source sequence,
        /// comparing them by the specified key projection.</returns>
        private static IEnumerable<TSource> DistinctByLocal<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var knownKeys = new HashSet<TKey>(comparer);
                foreach (var element in source)
                {
                    if (knownKeys.Add(keySelector(element)))
                    {
                        yield return element;
                    }
                }
        }
    }
}
