using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

namespace Pathfinder.Core.Client.Scripting
{
	public class MatchWaitTokenHandler : TokenHandler
	{
		private IGameServer _gameServer;
		private IScriptLog _scriptLog;

		protected override void execute()
		{
			_scriptLog = Context.Get<IScriptLog>();
			_scriptLog.Log(Context.Name, "matchwait", Context.LineNumber);

			_gameServer = Context.Get<IGameServer>();
			_gameServer.GameState.TextLog += Check;
		}

		private void Check(string text)
		{
			var matches = Context.MatchWait.Matches().ToList();

			foreach(var item in matches)
			{
				bool match = false;
				string matchedText = item.Pattern;
				if(item.IsRegex) {
					var regexMatch = Regex.Match(text, item.Pattern);
					match = regexMatch.Success;
					matchedText = regexMatch.Value;
				}
				else if(text.Contains(item.Pattern)) {
					match = true;
				}

				if(match)
				{
					_gameServer.GameState.TextLog -= Check;
					Context.MatchWait.Clear();
					DelayIfRoundtime(() => {
						_scriptLog.Log(Context.Name, "match goto " + item.Goto, Context.LineNumber);
						TaskSource.SetResult(new CompletionEventArgs { Goto = item.Goto });
					});
					break;
				}
			}
		}

		private void DelayIfRoundtime(Action complete)
		{
			double roundTime;
			if(double.TryParse(_gameServer.GameState.Get(ComponentKeys.Roundtime), out roundTime) && roundTime > 0)
			{
				DelayEx.Delay(TimeSpan.FromSeconds(roundTime), Context.CancelToken, complete);
			}
			else {
				complete();
			}
		}
	}
}
