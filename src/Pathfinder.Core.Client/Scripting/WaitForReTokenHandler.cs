using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Client.Scripting
{
	public class WaitForReTokenHandler : WeakMatchingTokenHandler
	{
		public WaitForReTokenHandler(IGameState gameState)
			: base(gameState)
		{
		}

		public override Task<CompletionEventArgs> Execute(ScriptContext context, Token token)
		{
			context.Get<IScriptLog>().Log(context.Name, "waitforre " + token.Value, context.LineNumber);

			return base.Execute(context, token);
		}

		protected override IWaitForMatcher BuildMatcher(ScriptContext context, Token token)
		{
			return new WaitforRegexMatcher(context, token);
		}
	}
}
