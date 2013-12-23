using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pathfinder.Core.Client.Scripting
{
	public class WaitForTokenHandler : TokenHandler
	{
		private IGameServer _gameServer;

		protected override void execute()
		{
			Context.Get<IScriptLog>().Log(Context.Name, "waiting for " + Token.Value, Context.LineNumber);

			_gameServer = Context.Get<IGameServer>();
			_gameServer.GameState.TextLog += Check;
		}

		private void Check(string text)
		{
			if(text.Contains(Token.Value)) {
				_gameServer.GameState.TextLog -= Check;
				DelayIfRoundtime();
				TaskSource.SetResult(new CompletionEventArgs());
			}
		}

		private void DelayIfRoundtime()
		{
			double roundTime;
			if(double.TryParse(_gameServer.GameState.Get(ComponentKeys.Roundtime), out roundTime) && roundTime > 0)
			{
				var task = Task.Delay(TimeSpan.FromSeconds(roundTime), Context.CancelToken);
				task.Wait();
			}
		}
	}

	public class MatchTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var matchToken = (MatchToken)Token;

			Context.Get<IScriptLog>().Log(Context.Name, "adding match for '" + matchToken.Pattern + "'", Context.LineNumber);

			Context.MatchWait.Add(matchToken);

			TaskSource.SetResult(new CompletionEventArgs());
		}
	}

	public class MatchWaitTokenHandler : TokenHandler
	{
		private IGameServer _gameServer;
		private IScriptLog _scriptLog;

		protected override void execute()
		{
			_scriptLog = Context.Get<IScriptLog>();
			_scriptLog.Log(Context.Name, "waiting for match", Context.LineNumber);

			_gameServer = Context.Get<IGameServer>();
			_gameServer.GameState.TextLog += Check;
		}

		private void Check(string text)
		{
			var matches = Context.MatchWait.Matches().ToList();

			foreach(var item in matches)
			{
				bool match = false;
				if(item.IsRegex) {
					match = Regex.Match(text, item.Pattern).Success;
				}
				else if(text.Contains(item.Pattern)) {
					match = true;
				}

				if(match)
				{
					_scriptLog.Log(Context.Name, "matched '" + item.Pattern + "'", Context.LineNumber);
					_gameServer.GameState.TextLog -= Check;
					Context.MatchWait.Clear();
					DelayIfRoundtime();
					TaskSource.SetResult(new CompletionEventArgs { Goto = item.Goto });
					break;
				}
			}
		}

		private void DelayIfRoundtime()
		{
			double roundTime;
			if(double.TryParse(_gameServer.GameState.Get(ComponentKeys.Roundtime), out roundTime) && roundTime > 0)
			{
				var task = Task.Delay(TimeSpan.FromSeconds(roundTime), Context.CancelToken);
				task.Wait();
			}
		}
	}
}
