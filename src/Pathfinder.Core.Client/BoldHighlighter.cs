using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Client
{
	public class BoldHighlighter : IHighlighter
	{
		private const string Pattern = "(<pushBold\\/>(.*?)<popBold\\/>)";

		private HighlightSettings _settings;
		public Action<TextTag, Match> Modify { get; set; }

		public BoldHighlighter(HighlightSettings settings)
		{
			_settings = settings;

			Modify = (tag, match) => {
				var setting = _settings.Get(HighlightKeys.Bold);
				tag.Text = match.Groups[2].Value;
				tag.Color = setting.Color;
				tag.Mono = setting.Mono;
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
				TextTag.For(text.Text.Substring(start, match.Index - start), text).IfNotEmpty(tags.Add);
				start = match.Index + match.Length;
				var matchedTag = TextTag.For(match.Groups[1].Value, text);
				Modify(matchedTag, match);
				matchedTag.IfNotEmpty(tags.Add);
			}

			if(start < text.Text.Length)
			{
				TextTag.For(text.Text.Substring(start, text.Text.Length - start), text).IfNotEmpty(tags.Add);
			}

			return tags;
		}
	}
}
