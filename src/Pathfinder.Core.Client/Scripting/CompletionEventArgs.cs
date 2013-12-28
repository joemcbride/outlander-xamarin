using System;

namespace Pathfinder.Core.Client
{
	public class CompletionEventArgs : EventArgs
	{
		public string Goto { get; set; }
		public string[] Args { get; set; }
		public Exception Error { get; set; }
	}	
}
