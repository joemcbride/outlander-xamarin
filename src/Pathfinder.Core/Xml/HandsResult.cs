using System;
using System.Xml.Linq;

namespace Pathfinder.Core
{
	public class HandsResult : ParseResult
	{
		public string Name { get; set; }
		public string Noun { get; set; }
		public string ItemId { get; set; }

		public static TItem For<TItem>(string xml) where TItem : HandsResult, new()
		{
			var result = new TItem();
			result.Matched = xml;

			var element = XElement.Parse(xml.Trim());

			result.Name = element.Value;
			result.Noun = element.Attribute("noun").Value;
			result.ItemId = element.Attribute("exist").Value;

			return result;
		}
	}

	public class LeftHandResult : HandsResult
	{
	}

	public class RightHandResult : HandsResult
	{
	}
}
