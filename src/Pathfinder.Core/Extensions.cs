using System;
using System.Collections.Generic;
using System.Net;

namespace Pathfinder.Core
{
    public static class Extensions
    {
		public static string ToFormat(this string format, params object[] args)
		{
			return string.Format(format, args);
		}

		public static T As<T>(this object item) where T : class
		{
			return item as T;
		}

		public static string EnsureEmpty(this string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return string.Empty;

			return value;
		}

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

		public static double DateTimeToUnixTimestamp(this DateTime dateTime)
		{
			TimeSpan span = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0,DateTimeKind.Utc));
			return span.TotalSeconds;
		}

		public static IPAddress GetIPAddress(this string address)
		{
			IPAddress ipAddress = null;

			if (IPAddress.TryParse(address, out ipAddress))
			{
				return ipAddress;
			}
			else
			{
				IPHostEntry ipHostInfo = Dns.GetHostEntry(address);
				return ipHostInfo.AddressList[ipHostInfo.AddressList.Length - 1];
			}
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

		/// <summary>
		/// Applies the action to each element in the list.
		/// </summary>
		/// <typeparam name="T">The enumerable item's type.</typeparam>
		/// <param name="enumerable">The elements to enumerate.</param>
		/// <param name="action">The action to apply to each item in the list.</param>
		public static void Apply<T>(this IEnumerable<T> enumerable, Action<T, int> action) {
			int i = 0;
			foreach(var item in enumerable) {
				action(item, i);
				i++;
			}
		}

		public static void IfNotNull<T>(this T item, Action<T> action)
		{
			if (item != null)
				action(item);
		}

		public static void IfNotNullOrEmpty(this string item, Action<string> action)
		{
			if (!string.IsNullOrWhiteSpace(item))
				action(item);
		}
    }
}
