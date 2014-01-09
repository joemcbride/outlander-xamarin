using System;

namespace Outlander.Core
{
	public class LamdaReporter<T> : DataReporter<T>
	{
		protected readonly Action<T> _listener;

		public LamdaReporter(Action<T> listener)
			: base(Guid.NewGuid().ToString())
		{
			_listener = listener;
		}

		public override void OnNext(T value)
		{
			_listener(value);
		}
	}
}
