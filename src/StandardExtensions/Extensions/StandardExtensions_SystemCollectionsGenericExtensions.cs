using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    /// <summary>
    /// Contains extension methods for types in the System.IO namespace.
    /// </summary>
    public static class StandardExtensions_SystemCollectionsGenericExtensions
    {
        private static readonly Random _rng = new Random();

        /// <summary>
        /// Compares this byte array with another.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="bytes">The byte array to compare with.</param>
        /// <returns>
        /// True if the arrays are identical.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">bytes</exception>
        public static bool BytesEqual(this byte[] a, byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");


            if (a.Length != bytes.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
                if (a[i] != bytes[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Splits a collection into smaller collections containing the specified number of elements each.
        /// </summary>
        /// <typeparam name="T">An IList type.</typeparam>
        /// <param name="instance"></param>
        /// <param name="elementsPerPart">The max number of elements from the original that the new collections should contain.</param>
        /// <returns>A Collection of IList</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "x"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> instance, int elementsPerPart)
        {
            // Copy to List so we can access the []indexer
            //
            var elements = new List<T>();
            foreach (var element in instance)
                elements.Add(element);

            var batches = new List<List<T>>();

            int i = 0;
            while (i < elements.Count)
            {
                var curBatch = new List<T>();
                foreach (int x in Enumerable.Range(1, elementsPerPart))
                {
                    if (i >= elements.Count)
                        break;

                    curBatch.Add(elements[i]);
                    i++;
                }
                batches.Add(curBatch);
            }
            return (IEnumerable<IEnumerable<T>>)batches;
        }

        /// <summary>
        /// Returns a null collection reference as an empty collection.
        /// </summary>
        public static IEnumerable<T> NullAsEmpty<T>(this IEnumerable<T> instance)
        {
            if (instance == null)
                return new List<T>();
            return instance;
        }

        /// <summary>
        /// Produces the set union of this sequence and a single other item by using the default equality comparer.
        /// </summary>
        public static IEnumerable<T> Union<T>(this IEnumerable<T> instance, T item)
        {
            return instance.Union(new T[] { item });
        }

        /// <summary>
        /// Determines whether a sequence contains no items.
        /// </summary>
        /// <typeparam name="T">The type of item in the sequence.</typeparam>
        /// <param name="instance"></param>
        /// <returns>True if no items match are in the sequence.</returns>
        public static Boolean None<T>(this IEnumerable<T> instance)
        {
            return !instance.Any<T>();
        }

        /// <summary>
        /// Determines whether a sequence contains no items.
        /// </summary>
        /// <typeparam name="T">The type of item in the sequence.</typeparam>
        /// <param name="instance"></param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>True if no items match the test condition.</returns>
        public static Boolean None<T>(this IEnumerable<T> instance, Func<T, bool> predicate)
        {
            return !instance.Any<T>(predicate);
        }

        /// <summary>
        /// Returns the next item in the list following the one specified.
        /// </summary>
        /// <returns>The next item or the default of T if there is no next item.</returns>
        public static T NextOrDefault<T>(this IList<T> instance, T item)
        {
            int i = instance.IndexOf(item) + 1;
            if (i == instance.Count || i == 0)
                return default(T);

            return instance[i];
        }

        /// <summary>
        /// Returns the previous item in the list following the one specified.
        /// </summary>
        /// <returns>The previous item or the default of T if there is no next item.</returns>
        public static T PreviousOrDefault<T>(this IList<T> instance, T item)
        {
            int i = instance.IndexOf(item) - 1;
            if (i < 0)
                return default(T);

            return instance[i];
        }

        /// <summary>
        /// Returns the key values as a HTTP query string to append to a Uri.
        /// </summary>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <param name="formUrlEncoded">Set to true to have the query string form-url-encoded.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static string ToHttpQueryString(this IEnumerable<KeyValuePair<string, string>> keyValuePairs, bool formUrlEncoded)
        {
            var sb = new StringBuilder();
            foreach (var pair in keyValuePairs)
            {
                if (String.IsNullOrWhiteSpace(pair.Key))
                    throw new InvalidOperationException(String.Format("A supplied key is empty. It's value is '{0}'", pair.Value));

                if (pair.Value.IsNotNullOrWhitespace())
                {
                    sb.Append(formUrlEncoded ? Uri.EscapeDataString(pair.Key) : pair.Key);
                    sb.Append("=");
                    sb.Append(formUrlEncoded ? Uri.EscapeDataString(pair.Value) : pair.Value);
                    sb.Append("&");
                }
            }
            string pre = sb.ToString();
            return pre.Substring(0, pre.Length - 1); // Remove last &.
        }

        /// <summary>
        /// Gets the value or returns null if the key doesn't exist.
        /// </summary>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <param name="key">The key to get the value for.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static T GetValueOrNull<T>(this IEnumerable<KeyValuePair<T, T>> keyValuePairs, string key)
        {
            return keyValuePairs.FirstOrDefault(pair => pair.Key.Equals(key)).Value;
        }

        /// <summary>
        /// Returns the strings in the collection as a single CSV string.
        /// </summary>
        public static string ToDelimitedString(this IEnumerable<String> values)
        {
            return ToDelimitedString(values, ", ", "");
        }

        /// <summary>
        /// Returns the strings in the collection as a single delimited string.
        /// </summary>
        /// <param name="delimiter">The delimiter, such as a comma.</param>
        /// <param name="enclosedIn">Some character or string to enclose each value in.</param>        
        /// <param name="values">Instance</param>
        public static string ToDelimitedString(this IEnumerable<String> values, string delimiter, string enclosedIn)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var value in values)
            {
                sb.Append(enclosedIn);
                sb.Append(value);
                sb.Append(enclosedIn);
                sb.Append(delimiter);
            }

            string s = sb.ToString();
            if (s.Length > delimiter.Length)
                s = s.Substring(0, s.Length - delimiter.Length);

            return s;
        }

        /// <summary>
        /// Randomizes the order of the collection using the Fidher-Yates shuffle.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The items in random order.</returns>
        public static IEnumerable<T> RandomOrder<T>(this IEnumerable<T> instance)
        {
            // http://en.algoritmy.net/article/43676/Fisher-Yates-shuffle
            
            var array = instance.ToArray();

            for (int i = array.Length; i > 1; i--)
            {
                int j = _rng.Next(i);

                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }

            return array;
        }

        /// <summary>
        /// Returns a new dictionary with values in ascending order according to their default IComparer implementation.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>Dictionary&lt;TKey, TValue&gt;.</returns>
        public static Dictionary<TKey, TValue> OrderByValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {

            var sortedPairs = dictionary.OrderBy(pair => pair.Value);
            return sortedPairs.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <summary>
        /// Orders the source sequence by each items appearance in the second ordered sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TOrdered">The type of the t ordered.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="ordered">The ordered sequence.</param>
        /// <param name="sourceKeySelector">A function that selects a key from each item in the source sequence to look for in the second.</param>
        /// <returns>IOrderedEnumerable&lt;TSource&gt;.</returns>
        public static IOrderedEnumerable<TSource> OrderByAppearanceIn<TSource, TOrdered>(this IEnumerable<TSource> source, IEnumerable<TOrdered> ordered, Func<TSource, TOrdered> sourceKeySelector)
        {
            var orderedList = ordered.ToList();

            return source.OrderBy(i => orderedList.IndexOf(sourceKeySelector(i)));
        }

        /// <summary>
        /// Returns the only item in the sequence or a default value if more or less than one item is enumerated.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public static T OneOrDefault<T>(this IEnumerable<T> collection)
        {
            var e = collection.GetEnumerator();
            if (e.MoveNext())
            {
                // One

                T current = e.Current;

                if (e.MoveNext())
                {
                    // Two

                    return default(T);
                }
                else
                {
                    return current;
                }
            }
            else
            {
                // None.

                return default(T);
            }
        }

        /// <summary>
        /// Returns the set of items, made distinct by the selected value.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">A function that selects a value to determine unique results.</param>
        /// <returns>IEnumerable&lt;TSource&gt;.</returns>
        public static IEnumerable<TSource> Distinct<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            HashSet<TResult> set = new HashSet<TResult>();

            foreach (var item in source)
            {
                var selectedValue = selector(item);

                if (set.Add(selectedValue))
                    yield return item;
            }
        }

        /// <summary>
        /// Creates an array but is returned in its IEnumerable guise to avoid an extra cast if the compiler is struggling to infer.
        /// </summary>
        /// <typeparam name="T">The type of elements in the source collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>An enumerable collection of items.</returns>
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerable<T> source)
        {
            return (IEnumerable<T>)source.ToArray<T>();
        }

        /// <summary>
        /// Iterates and touches each item in the collection and returns the full collection for further processing.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="action">The action to run on each item.</param>
        /// <returns></returns>
        public static IEnumerable<T> VisitEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);

            return source;
        }

        /// <summary>
        /// Tries to get the first item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="first">The first.</param>
        /// <returns></returns>
        public static bool TryFirst<T>(this IEnumerable<T> source, out T first)
        {
            first = source.FirstOrDefault();
            return first != null;
        }

        /// <summary>
        /// Tries to get the first item matching a given predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="first">The first.</param>
        /// <returns></returns>
        public static bool TryFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate, out T first)
        {
            first = source.FirstOrDefault(predicate);
            return first != null;
        }

        /// <summary>
        /// Returns only the its in even-numbered elements, 0, 2, 4, 6 etc.
        /// </summary>
        public static IEnumerable<T> Evens<T>(this IEnumerable<T> source)
        {
            long c = source.LongCount();
            long m = (c % 2 == 0) ? c / 2 : c / 2 + 1;
            T[] bucket = new T[m];

            int a = 0;
            int i = 0;
            foreach (var item in source)
            {
                if (i++ % 2 == 0)
                {
                    // Even

                    bucket[a++] = item;
                }
            }

            return bucket;
        }

        /// <summary>
        /// Returns only the its in odd-numbered elements.
        /// </summary>
        public static IEnumerable<T> Odds<T>(this IEnumerable<T> source)
        {
            long c = source.LongCount();
            long m = (c % 2 == 0) ? c / 2 : c / 2 + 1;
            T[] bucket = new T[m];

            int a = 0;
            int i = 0;
            foreach (var item in source)
            {
                if (i++ % 2 != 0)
                {
                    // Odd

                    bucket[a++] = item;
                }
            }

            return bucket;
        }

        /// <summary>
        /// Returns items that are duplicated in the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> source)
        {
            bool hasValue = false;
            T current = default(T);
            foreach (var item in source.Where(i => i != null).OrderBy(i => i))
            {
                if (hasValue && current.Equals(item))
                    yield return item;

                current = item;
                hasValue = true;
            }
        }

        /// <summary>
        /// Returns items that are duplicated in the collection.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public static IEnumerable<TSource> Duplicates<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            bool hasValue = false;
            TResult lastValue = default(TResult);
            foreach (var item in source.Where(i => i != null).OrderBy(selector))
            {
                var selectedValue = selector(item);
                if (hasValue && lastValue.Equals(selectedValue))
                    yield return item;

                lastValue = selectedValue;
                hasValue = true;
            }
        }
    }
}
