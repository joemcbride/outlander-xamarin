using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Outlander.Core.Client;
using Outlander.Core;

namespace Outlander.Core.Client.Scripting
{
	public class WaitForReTokenHandler : WeakMatchingTokenHandler
	{
		public WaitForReTokenHandler(IGameState gameState, IGameStream gameStream)
			: base(gameState, gameStream)
		{
		}

		public override Task<CompletionEventArgs> Execute(ScriptContext context, Token token)
		{
			if(context.DebugLevel > 0) {
				context.Get<IScriptLog>().Log(context.Name, "waitforre " + token.Value, context.LineNumber);
			}

			return base.Execute(context, token);
		}

		protected override IWaitForMatcher BuildMatcher(ScriptContext context, Token token)
		{
			return new WaitforRegexMatcher(context, token);
		}
	}
}
