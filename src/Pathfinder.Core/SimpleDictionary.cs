using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;

namespace Pathfinder.Core
{
	public interface ISimpleDictionary<TKey, TValue>
	{
		TValue this[TKey key] { get; set; }

		bool HasKey(TKey key);
		TValue Get(TKey key);
		void Set(TKey key, TValue value);
		void Remove(TKey key);
		IDictionary<TKey, TValue> Values();
	}

	public class SimpleDictionary<TKey, TValue> : ISimpleDictionary<TKey, TValue>
	{
		private IDictionary<TKey, TValue> _components;

		public SimpleDictionary()
			: this(new Dictionary<TKey, TValue>())
		{
		}

		public SimpleDictionary(IDictionary<TKey, TValue> values)
		{
			_components = new Dictionary<TKey, TValue>(values);
		}

		private static object LockObject = new object();

		public TValue this[TKey key]
		{
			get { return Get(key); }
			set { Set(key, value); }
		}

		public bool HasKey(TKey key)
		{
			lock (LockObject) {
				return _components.ContainsKey(key);
			}
		}

		public TValue Get(TKey key)
		{
			lock (LockObject) {
				if (_components.ContainsKey(key))
					return _components[key];

				return default(TValue);
			}
		}

		public void Set(TKey key, TValue value)
		{
			if(key == null || string.IsNullOrWhiteSpace(key.ToString()))
				return;

			lock (LockObject) {
				_components[key] = value;
				System.Diagnostics.Debug.WriteLine("Setting {0}::{1}", key, value);
			}
		}

		public void Remove(TKey key)
		{
			if(key == null || string.IsNullOrWhiteSpace(key.ToString()))
				return;
			lock(LockObject) {
				if(_components.ContainsKey(key))
					_components.Remove(key);
			}
		}

		public IDictionary<TKey, TValue> Values()
		{
			return new Dictionary<TKey, TValue>(_components);
		}
	}
}
