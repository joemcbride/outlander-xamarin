using System;
using System.Collections.Generic;
using NUnit.Framework;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;

namespace Pathfinder.Core.Tests
{
	public class InMemoryServiceLocator : IServiceLocator
	{
		private IDictionary<Type, object> _services = new Dictionary<Type, object>();

		public void Add<T>(T service)
		{
			_services.Add(typeof(T), service);
		}

		public T Get<T>()
		{
			var type = typeof(T);
			if(!_services.ContainsKey(type))
				throw new KeyNotFoundException("InMemoryServiceLocator::Unable to find key {0}".ToFormat(type));

			return (T)_services[type];
		}

		public IEnumerable<T> GetAll<T>()
		{
			throw new NotImplementedException();
		}
	}
}
