using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Pathfinder.Core.Client
{

	public class CompletionEventArgs : EventArgs
	{
		public string Goto { get; set; }
		public Exception Error { get; set; }
	}
	
}
