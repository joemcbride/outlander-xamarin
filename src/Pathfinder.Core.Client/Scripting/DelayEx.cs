using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

namespace Pathfinder.Core.Client.Scripting
{

	public class DelayEx
	{
		public static void Delay(TimeSpan delay, CancellationToken token, Action complete)
		{
			Task.Factory.StartNew(() => {
				var task = Task.Delay(delay, token);
				task.Wait();
				complete();
			});
		}
	}
	
}
