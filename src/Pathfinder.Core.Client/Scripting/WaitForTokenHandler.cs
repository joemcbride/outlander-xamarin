using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Client.Scripting
{
	public class WaitForTokenHandler : WeakMatchingTokenHandler
	{
		public WaitForTokenHandler(IGameState gameState)
			: base(gameState)
		{
		}

		public override Task<CompletionEventArgs> Execute(ScriptContext context, Token token)
		{
			context.Get<IScriptLog>().Log(context.Name, "waitfor " + token.Value, context.LineNumber);

			return base.Execute(context, token);
		}
	}
}
