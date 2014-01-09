using System;
using Outlander.Core;
using System.Threading;
using Outlander.Core.Client.Scripting;

namespace Outlander.Core.Client
{
	public static class GameStateExtensions
	{
		public static void DelayIfRoundtime(this IGameState gameState, CancellationToken cancelToken, Action complete, double offset = 0.0)
		{
			double roundTime;
			if(double.TryParse(gameState.Get(ComponentKeys.Roundtime), out roundTime) && roundTime > 0)
			{
				DelayEx.Delay(TimeSpan.FromSeconds(roundTime - offset), cancelToken, complete);
			}
			else {
				complete();
			}
		}
	}
}
