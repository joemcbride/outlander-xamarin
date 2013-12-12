using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Client
{
	public class TextTag
	{
		public string Text { get; set; }
		public string Color { get; set; }
		public bool Mono { get; set; }
		public bool Matched { get; set; }

		public override bool Equals(object obj)
		{
			return Equals((TextTag)obj);
		}

		public bool Equals(TextTag tag)
		{
			return tag.Text.Equals(tag.Text);
		}

		public override string ToString()
		{
			return string.Format("[TextTag: Text={0}, Color={1}, Mono={2}]", Text, Color, Mono);
		}

		public void IfNotEmpty(Action<TextTag> action)
		{
			if(!string.IsNullOrWhiteSpace(Text) && action != null)
				action(this);
		}

		public static TextTag For(string text)
		{
			return new TextTag { Text = text };
		}

		public static TextTag For(string text, TextTag tag)
		{
			return new TextTag { Text = text, Color = tag.Color, Mono = tag.Mono };
		}
	}
}
