using System;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Client.Scripting
{
	public class MatchWaitTokenHandler : WeakMatchingTokenHandler
	{
		public MatchWaitTokenHandler(IGameState gameState)
			: base(gameState)
		{
		}

		public override Task<CompletionEventArgs> Execute(ScriptContext context, Token token)
		{
			context.Get<IScriptLog>().Log(context.Name, "matchwait", context.LineNumber);

			return base.Execute(context, token);
		}

		protected override IWaitForMatcher BuildMatcher(ScriptContext context, Token token)
		{
			return new MatchWaitMatcher(context, token);
		}
	}

	public class MatchWaitMatcher : WaitForMatcher
	{
		public MatchWaitMatcher(ScriptContext context, Token token)
			: base(context, token)
		{
		}

		public override MatchResult Checkmatch(string text)
		{
			var matches = Context.MatchWait.Matches().ToList();

			var result = MatchResult.False();

			foreach(var item in matches)
			{
				var match = false;
				var matchedText = item.Pattern;
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
					Context.MatchWait.Clear();
					result = MatchResult.True(item.Goto);
					Context.Get<IScriptLog>().Log(Context.Name, "match goto " + item.Goto, Context.LineNumber);
					break;
				}
			}

			return result;
		}
	}
}
