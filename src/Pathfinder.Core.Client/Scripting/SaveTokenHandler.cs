using System;

namespace Pathfinder.Core.Client
{
	public class SaveTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			Context.Get<IScriptLog>().Log(Context.Name, "saving {0}".ToFormat(Token.Value), Context.LineNumber);
			Context.LocalVars.Set("s", Token.Value);
		}
	}
}
