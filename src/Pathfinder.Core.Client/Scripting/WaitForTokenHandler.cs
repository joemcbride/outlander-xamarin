using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Outlander.Core.Client;

namespace Pathfinder.Core.Client.Scripting
{
	public class WaitForTokenHandler : WeakMatchingTokenHandler
	{
		public WaitForTokenHandler(IGameState gameState, IGameStream gameStream)
			: base(gameState, gameStream)
		{
		}

		public override Task<CompletionEventArgs> Execute(ScriptContext context, Token token)
		{
			if(context.DebugLevel > 0) {
				context.Get<IScriptLog>().Log(context.Name, "waitfor " + token.Value, context.LineNumber);
			}

			return base.Execute(context, token);
		}
	}
}
