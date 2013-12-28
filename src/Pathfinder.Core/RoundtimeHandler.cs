using System;
using System.Timers;
using Interlocked = System.Threading.Interlocked;

namespace Pathfinder.Core
{
	public class RoundtimeArgs
	{
		public long Roundtime { get; set; }
		public bool Reset { get; set; }
	}

	public interface IRoundtimeHandler
	{
		event EventHandler<RoundtimeArgs> Changed;
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
				FireChanged(value, false);
				if(value <= 0)
					_timer.Stop();
			};
		}

		public event EventHandler<RoundtimeArgs> Changed = delegate { };

		public void Set(long roundTime)
		{
			Interlocked.Exchange(ref _roundTime, roundTime + 1);

			var value = Interlocked.Read(ref _roundTime);

			bool reset = false;
			if(!_timer.Enabled) {
				_timer.Start();
				reset = true;
			}

			FireChanged(value, reset);
		}

		private void FireChanged(long roundTime, bool reset)
		{
			if(roundTime < 0)
				roundTime = 0;

			_services.Get<IGameState>().Set(ComponentKeys.Roundtime, roundTime.ToString());
			var ev = Changed;
			if(ev != null)
			{
				ev(this, new RoundtimeArgs { Roundtime = roundTime, Reset = reset });
			}
		}
	}
}
