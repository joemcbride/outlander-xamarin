using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Pathfinder.Core.Client.Scripting;

namespace Pathfinder.Core.Client
{
	public interface ISendQueue
	{
		void Add(string command);
		void Clear();
		void ProcessQueue();
	}

	public class SendQueue : ISendQueue
	{
		private readonly Queue<string> _commandQueue;
		private readonly ICommandProcessor _commandProcessor;
		private readonly IGameState _gameState;

		public SendQueue(ICommandProcessor commandProcessor, IGameState gameState)
		{
			_commandQueue = new Queue<string>();
			_commandProcessor = commandProcessor;
			_gameState = gameState;
		}

		public void Add(string command)
		{
			_commandQueue.Enqueue(command);
			ProcessQueue();
		}

		public void Clear()
		{
			_commandQueue.Clear();
		}

		public void ProcessQueue()
		{
			DelayIfRoundtime(() => {
				while(_commandQueue.Count > 0) {
					_commandProcessor.Process(_commandQueue.Dequeue());
				}
			});
		}

		protected void DelayIfRoundtime(Action complete)
		{
			double roundTime;
			if(double.TryParse(_gameState.Get(ComponentKeys.Roundtime), out roundTime) && roundTime > 0)
			{
				DelayEx.Delay(TimeSpan.FromSeconds(roundTime - 0.25), CancellationToken.None, complete);
			}
			else {
				complete();
			}
		}
	}
}

