using System;

namespace Outlander.Core.Text
{
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
