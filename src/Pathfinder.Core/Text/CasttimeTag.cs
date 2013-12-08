using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text
{

	public class CasttimeTag : Tag
	{
		public DateTime RoundTime { get; set; }

		protected override void OnTextSet()
		{
			var element = XElement.Parse(Text.Trim());
			RoundTime = element.Attribute("value").Value.UnixTimeStampToDateTime();
		}
	}
	
}
