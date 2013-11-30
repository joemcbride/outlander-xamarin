using System;
using System.Xml.Linq;

namespace Pathfinder.Core
{
	public class RightHandParser : RegexParser<RightHandResult>
	{
		private const string Match_Regex = "<right(.*?)<\\/right>";

		public RightHandParser()
		{
			Pattern = Match_Regex;
		}

		protected override RightHandResult BuildItem(string xml)
		{
			return HandsResult.For<RightHandResult>(xml);
		}
	}	
}
