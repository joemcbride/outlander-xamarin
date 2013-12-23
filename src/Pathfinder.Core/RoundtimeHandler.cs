using System;
using System.Timers;
using Interlocked = System.Threading.Interlocked;

namespace Pathfinder.Core
{
	public interface IRoundtimeHandler
	{
		event EventHandler<long> Changed;
		void Set(long roundTime);
	}

	public class RoundtimeHandler : IRoundtimeHandler
	{
		private readonly Timer _timer;
		private readonly IServiceLocator _services;

		private long _roundTime;

		public RoundtimeHandler(IServiceLocator services)
		{
			_services = services;
			_timer = new Timer();
			_timer.Interval = 1000;
			_timer.Elapsed += (sender, e) =>
			{
				Interlocked.Decrement(ref _roundTime);
				var value = Interlocked.Read(ref _roundTime);
				FireChanged(value);
				if(value == 0)
					_timer.Stop();
			};
		}

		public event EventHandler<long> Changed = delegate { };

		public void Set(long roundTime)
		{
			Interlocked.Add(ref _roundTime, roundTime);

			var value = Interlocked.Read(ref _roundTime);
			FireChanged(value);

			if(!_timer.Enabled)
				_timer.Start();
		}

		private void FireChanged(long roundTime)
		{
			_services.Get<IGameState>().Set(ComponentKeys.Roundtime, roundTime.ToString());
			var ev = Changed;
			if(ev != null)
			{
				ev(this, roundTime);
			}
		}
	}
}
