using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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
			: this(null)
		{
		}

		public SimpleDictionary(IDictionary<TKey, TValue> values)
		{
			if(values == null) {
				_components = new ConcurrentDictionary<TKey, TValue>();
			}
			else {
				_components = new ConcurrentDictionary<TKey, TValue>(values);
			}
		}

		public TValue this[TKey key]
		{
			get { return Get(key); }
			set { Set(key, value); }
		}

		public bool HasKey(TKey key)
		{
			return _components.ContainsKey(key);
		}

		public TValue Get(TKey key)
		{
			if(_components.ContainsKey(key))
				return _components[key];

			return default(TValue);
		}

		public void Set(TKey key, TValue value)
		{
			if(key == null || string.IsNullOrWhiteSpace(key.ToString()))
				return;

			_components[key] = value;
			System.Diagnostics.Debug.WriteLine("Setting {0}::{1}", key, value);
		}

		public void Remove(TKey key)
		{
			if(key == null || string.IsNullOrWhiteSpace(key.ToString()))
				return;

			if(_components.ContainsKey(key))
				_components.Remove(key);
		}

		public IDictionary<TKey, TValue> Values()
		{
			return _components.ToDictionary(x => x.Key, x => x.Value);
		}
	}
}
