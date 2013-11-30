using System;
using System.Xml.Linq;

namespace Pathfinder.Core
{

	public class LeftHandParser : RegexParser<LeftHandResult>
	{
		private const string Match_Regex = "<left(.*?)<\\/left>";

		public LeftHandParser()
		{
			Pattern = Match_Regex;
		}

		protected override LeftHandResult BuildItem(string xml)
		{
			return HandsResult.For<LeftHandResult>(xml);
		}
	}
	
}
