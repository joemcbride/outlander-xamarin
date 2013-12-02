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

	public class Tag
	{
		private string _text;

		public Tag()
		{
		}

		public Tag(string text)
		{
			Text = text;
		}

		public string Text {
			get {
				return _text;
			}
			private set {
				_text = value;
				OnTextSet();
			}
		}

		protected virtual void OnTextSet()
		{
		}

		public static TTag For<TTag>(string text) where TTag : Tag, new()
		{
			return new TTag(){ Text = text };
		}
	}	
}
