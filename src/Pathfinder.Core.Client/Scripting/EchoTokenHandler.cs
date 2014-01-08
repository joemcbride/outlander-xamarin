using System;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Client
{
	public class EchoTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var commandProcessor = Context.Get<ICommandProcessor>();

			var echo = Token.Value;

			commandProcessor.Echo(echo, Context);

			TaskSource.SetResult(new CompletionEventArgs());
		}
	}
}
