using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Pathfinder.Core.Text
{

//	public static class StringExtensions
//	{
//		public static bool StartsWithAllChars(this string text, string value, int minLength)
//		{
//			if(value.Length > text.Length)
//			{
//				//var length = text.Length <= minLength ? text.Length : minLength;
//				value = value.Substring(0, minLength);
//			}
//
//			return text.StartsWith(value);
//		}
//	}

	public class PromptTag : Tag
	{
		public DateTime? Time { get; set; }
		public string Prompt { get; set; }

		protected override void OnTextSet()
		{
			var element = XElement.Parse(Text.Trim());
			Time = element.Attribute("time").Value.UnixTimeStampToDateTime();
			Prompt = element.Value;
		}
	}
	
}
