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

	public class Chunk
	{
		public Chunk(string text)
		{
			Text = text;
		}

		public string Text { get; private set; }

		public override string ToString()
		{
			return string.Format("[Chunk: Text={0}]", Text);
		}

		public static Chunk For(string text, int start = 0, int length = -1)
		{
			if (length == -1)
				length = text.Length;

			return new Chunk(text.Substring(start, length));
		}
	}
}
