using System;
using System.Collections.Generic;

namespace Pathfinder.Core
{
    public static class Extensions
    {
        //public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T> eachAction)
        //{
        //    foreach (T obj in values)
        //        eachAction(obj);
        //    return values;
        //}

		public static DateTime UnixTimeStampToDateTime(this string unixTimeStamp)
		{
			return UnixTimeStampToDateTime (double.Parse (unixTimeStamp));
		}

        public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        /// <summary>
        /// Applies the action to each element in the list.
        /// </summary>
        /// <typeparam name="T">The enumerable item's type.</typeparam>
        /// <param name="enumerable">The elements to enumerate.</param>
        /// <param name="action">The action to apply to each item in the list.</param>
        public static void Apply<T>(this IEnumerable<T> enumerable, Action<T> action) {
            foreach(var item in enumerable) {
                action(item);
            }
        }

		public static void IfNotNull<T>(this T item, Action<T> action)
		{
			if (item != null)
				action(item);
		}
    }
}
