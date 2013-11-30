using System;

namespace Pathfinder.Core
{
	public class ParseResult
	{
		public string Matched { get; set;}

		public virtual bool IsValid()
		{
			return false;
		}
	}
}