using System;

namespace Pathfinder.Core.Client
{
	public class ExitTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			TaskSource.TrySetCanceled();
		}
	}
}
