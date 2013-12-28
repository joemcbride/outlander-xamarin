using System;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Client
{
	public class EchoTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var commandProcessor = Context.Get<ICommandProcessor>();

			commandProcessor.Echo(Token.Value, Context);

			TaskSource.SetResult(new CompletionEventArgs());
		}
	}
}
