using System;

namespace Pathfinder.Core.Client
{

	public class HasVarTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			var values = Token.Value.Split(new string[]{ " " }, StringSplitOptions.None);

			Context.LocalVars.Set(values[1], Context.LocalVars.HasKey(values[0]).ToString());

			TaskSource.TrySetResult(new CompletionEventArgs());
		}
	}
}
