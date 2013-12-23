using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

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
				DelayIfRoundtime(() => TaskSource.SetResult(new CompletionEventArgs()));
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
