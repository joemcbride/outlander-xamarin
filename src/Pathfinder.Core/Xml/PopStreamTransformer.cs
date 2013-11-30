using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Pathfinder.Core
{
	public class PopStreamTransformer : ITransformer
	{
		public bool Matches(string data)
		{
			return !string.IsNullOrWhiteSpace(data) && data.Contains("pushStream") && data.Contains("<popStream/>");
		}

		public string Transform(string data)
		{
			var replaced = data.Replace("<popStream/>", "</pushStream>");

			var matches = Regex.Matches(replaced, "<pushStream(.*?)\\/>");

			foreach(Match match in matches)
			{
				replaced = replaced.Replace(match.Value, match.Value.Replace("/>", ">"));
			}

			return replaced;
		}
	}
}
