using System;
using Pathfinder.Core;
using Pathfinder.Core.Client;

namespace Outlander.Core.Client
{
	public interface IGameStream : IObservable<TextTag>
	{
		void Publish(TextTag tag);
	}

	public class GameStream : DataTracker<TextTag>, IGameStream
	{
		private readonly LamdaReporter<string> _listener;

		public GameStream(IGameState gameState)
		{
			_listener = new LamdaReporter<string>(data => {
				Publish(TextTag.For(data));
			});

			_listener.Subscribe(gameState.TextTracker);
		}
	}

	public interface IGameStreamListener
	{
	}

	public class GameStreamListener : LamdaReporter<TextTag>, IGameStreamListener
	{
		public GameStreamListener(Action<TextTag> listener)
			: base(listener)
		{
		}
	}

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
