using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Pathfinder.Core.Text
{
	public class PromptTag : Tag
	{
		public DateTime Time { get; set; }
		public string GameTime { get; set; }
		public string Prompt { get; set; }

		protected override void OnTextSet()
		{
			var element = XElement.Parse(Text.Trim());

			GameTime = element.Attribute("time").Value;
			Time = GameTime.UnixTimeStampToDateTime();
			Prompt = element.Value;
		}
	}
}
