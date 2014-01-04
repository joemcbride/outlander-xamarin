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
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((TextTag)obj);
		}

		public bool Equals(TextTag other)
		{
			return string.Equals(Text, other.Text);
		}

		public override int GetHashCode()
		{
			return (Text != null ? Text.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return string.Format("[TextTag: Text={0}, Color={1}, Mono={2}]", Text, Color, Mono);
		}

		public void IfNotNull(Action<TextTag> action)
		{
			if(Text != null && Text != string.Empty && action != null)
				action(this);
		}

		public static TextTag For(string text)
		{
			return For(text, string.Empty);
		}

		public static TextTag For(string text, string color)
		{
			return For(text, color, false);
		}

		public static TextTag For(string text, TextTag tag)
		{
			return For(text, tag.Color, tag.Mono);
		}

		public static TextTag For(string text, string color, bool mono)
		{
			return new TextTag { Text = text, Color = color, Mono = mono };
		}
	}
}
