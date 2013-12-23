using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text
{
	public class StreamWindowTag : Tag
	{
		private const string Id_Regex = "id=\\'(\\w+)\\'";
		private const string Title_Regex = "title=\\'(\\w+)\\'";
		private const string SubTitle_Regex = "subtitle=\\\"(.*)\\\"";

		public string Id { get; set; }
		public string Title { get; set; }
		public string Subtitle { get; set; }

		protected override void OnTextSet()
		{
			Id = GetValue(Text, Id_Regex);
			Title = GetValue(Text, Title_Regex);
			Subtitle = GetValue(Text, SubTitle_Regex);

//			var element = XElement.Parse(Text.Trim());
//			Id = element.Attribute("id").Value.Replace(" ", string.Empty).ToLower();
//			element.Attribute("title").IfNotNull(e => Title = e.Value);
//			element.Attribute("subtitle").IfNotNull(e => Subtitle = e.Value);
		}

		private string GetValue(string text, string regex)
		{
			var value = string.Empty;

			var match = Regex.Match(text, regex);
			if(match.Success)
				value = match.Groups[1].Value;

			return value;
		}
	}
}
