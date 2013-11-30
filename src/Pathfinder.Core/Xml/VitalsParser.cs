using System;
using System.Linq;
using System.Xml.Linq;

namespace Pathfinder.Core
{
	public class VitalsParser : RegexParser<VitalsResult>
	{
		private const string Match_Regex = "<dialogData(.*?)<\\/dialogData>";

		public VitalsParser()
		{
			Pattern = Match_Regex;
		}

		protected override VitalsResult BuildItem(string xml)
		{
			return VitalsResult.For(xml);
		}
	}

	public class VitalsResult : ParseResult
	{
		public string Name { get; set; }
		public int Value { get; set; }

		public static VitalsResult For(string xml)
		{
			var result = new VitalsResult();
			result.Matched = xml;

			var element = XElement.Parse(xml.Trim());
			var progressBar = GetElement(element, "progressBar");

			if(progressBar != null)
			{
				result.Name = progressBar.Attribute("id").Value;
				result.Value = int.Parse(progressBar.Attribute("value").Value);
			}

			return result;
		}

		private static XElement GetElement(XElement element, string xName)
		{
			return element
				.Elements()
					.Where(x => x.Name == XName.Get(xName))
				.FirstOrDefault();
		}
	}
}
