using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Pathfinder.Core.Xml
{
	public class GenericStreamParser : RegexParser<StreamResult>
	{
		private const string Match_Regex = "<pushStream(.*?)\n?<\\/pushStream>";

		public GenericStreamParser()
		{
			Pattern = Match_Regex;
		}

		protected override StreamResult BuildItem(string xml)
		{
			return StreamResult.For<StreamResult>(xml);
		}
	}	
}
