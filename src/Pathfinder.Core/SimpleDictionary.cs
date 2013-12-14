using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;

namespace Pathfinder.Core
{
	public class SimpleDictionary
	{
		private IDictionary<string, string> _components = new Dictionary<string, string>();

		private static object LockObject = new object();

		public bool HasKey(string key)
		{
			lock (LockObject) {
				return _components.ContainsKey(key);
			}
		}

		public string Get(string key)
		{
			lock (LockObject) {
				if (_components.ContainsKey(key))
					return _components[key];

				return null;
			}
		}

		public void Set(string key, string value)
		{
			if (string.IsNullOrWhiteSpace(key))
				return;

			lock (LockObject) {
				_components[key] = value;
				System.Diagnostics.Debug.WriteLine("Setting {0}::{1}", key, value);
			}
		}
	}
}
