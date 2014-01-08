using System;
using Pathfinder.Core.Client;

namespace Outlander.Core.Client
{
	public class ParseTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var command = Context.Get<ICommandProcessor>();
			command.Parse(Token.Value, Context);

			TaskSource.TrySetResult(new CompletionEventArgs());
		}
	}
}
