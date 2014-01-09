using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Outlander.Core.Client
{
	public class MonoHighlighter : IHighlighter
	{
		private const string Pattern = "(<output class=\"mono\"\\/>(.*?)<output class=\"\"/>)";

		public Action<TextTag, Match> Modify { get; set; }

		public MonoHighlighter()
		{
			Modify = (tag, match) => {
				tag.Text = match.Groups[2].Value;
				tag.Mono = true;
				tag.Matched = true;
			};
		}

		public IEnumerable<TextTag> Highlight(TextTag text)
		{
			var tags = new List<TextTag>();

			var start = 0;

			var matches = Regex.Matches(text.Text, Pattern, RegexOptions.Singleline);
			foreach(Match match in matches)
			{
				TextTag.For(text.Text.Substring(start, match.Index - start), text).IfNotNull(tags.Add);
				start = match.Index + match.Length;
				var matchedTag = TextTag.For(match.Groups[1].Value, text);
				Modify(matchedTag, match);
				matchedTag.IfNotNull(tags.Add);
			}

			if(start < text.Text.Length)
			{
				TextTag.For(text.Text.Substring(start, text.Text.Length - start), text).IfNotNull(tags.Add);
			}

			return tags;
		}
	}
	
}
