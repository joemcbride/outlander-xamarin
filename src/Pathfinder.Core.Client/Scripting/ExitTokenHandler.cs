using System;

namespace Outlander.Core.Client
{
	public class ExitTokenHandler : TokenHandler
	{
		protected override void execute()
		{
			TaskSource.TrySetCanceled();
		}
	}
}
