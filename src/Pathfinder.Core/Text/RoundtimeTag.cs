using System;
using System.Xml.Linq;

namespace Outlander.Core.Text
{
	public class RoundtimeTag : Tag
	{
		public string Value { get; set; }
		public DateTime RoundTime { get; set; }

		protected override void OnTextSet()
		{
			var element = XElement.Parse(Text.Trim());
			Value = element.Attribute("value").Value;
			RoundTime = Value.UnixTimeStampToDateTime();
		}
	}
}
