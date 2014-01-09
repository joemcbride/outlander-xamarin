using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Outlander.Core.Client.Scripting;
using Outlander.Core.Client;
using Outlander.Core;

namespace Outlander.Core.Client
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
			_gameState.DelayIfRoundtime(CancellationToken.None, () => {
				while(_commandQueue.Count > 0) {
					_commandProcessor.Process(_commandQueue.Dequeue());
				}
			}, 0.25);
		}
	}
}

