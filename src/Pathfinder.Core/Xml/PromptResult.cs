using System;
using System.Xml.Linq;

namespace Pathfinder.Core.Xml
{
	public class PromptResult : ParseResult
	{
		public DateTime? Time { get; set; }
		public string Prompt { get; set; }

		public override bool IsValid ()
		{
			return Time.HasValue && !string.IsNullOrWhiteSpace (Prompt);
		}

		public static PromptResult For(string xml)
		{
			var prompt = new PromptResult();

			var element = XElement.Parse(xml.Trim());
			prompt.Time = element.Attribute("time").Value.UnixTimeStampToDateTime();
			prompt.Prompt = element.Value;
			prompt.Matched = xml;

			return prompt;
		}
	}
}
