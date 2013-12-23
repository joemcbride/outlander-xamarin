using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Pathfinder.Core.Client
{
	public class PauseTokenHandler : TokenHandler
	{
		private IScriptLog _log;
		private IGameState _gameState;

		protected override void execute()
		{
			_log = Context.Get<IScriptLog>();
			_gameState = Context.Get<IGameState>();

			Pause(Token);
		}

		public void Pause(Token token)
		{
			double pause = 1.0;
			if(!string.IsNullOrWhiteSpace(token.Value))
				double.TryParse(token.Value, out pause);

			_log.Log(Context.Name, "pausing for {0} seconds".ToFormat(pause), Context.LineNumber);

			try
			{
				var pauseTime = TimeSpan.FromSeconds(pause);
				var task = Task.Delay(pauseTime, Context.CancelToken);
				task.Wait();

				double roundTime;
				if(double.TryParse(_gameState.Get(ComponentKeys.Roundtime), out roundTime) && roundTime > 0)
				{
					task = Task.Delay(TimeSpan.FromSeconds(roundTime), Context.CancelToken);
					task.Wait();
				}
				TaskSource.TrySetResult(new CompletionEventArgs());
			}
			catch(AggregateException)
			{
				TaskSource.TrySetCanceled();
			}
		}
	}
}
