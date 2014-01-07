using System;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Client
{
	public class EchoTokenHandler : TokenHandler
	{
		private const string Mono_Template = "<output class=\"mono\"/>{0}<output class=\"\"/>";

		protected override void execute()
		{
			var commandProcessor = Context.Get<ICommandProcessor>();

			var echo = Token.Value;

			if(!string.IsNullOrWhiteSpace(Token.Value))
			{
				echo = Mono_Template.ToFormat(echo);
			}

			commandProcessor.Echo(echo, Context);

			TaskSource.SetResult(new CompletionEventArgs());
		}
	}
}
