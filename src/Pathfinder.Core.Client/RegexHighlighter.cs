using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Client
{
	public class RegexHighlighter : IHighlighter
	{
		public Func<string, bool> Matches { get; set; }
		public Action<TextTag> Modify { get; set; }

		protected TextTag _targetTag;

		public virtual string BuildPattern()
		{
			return string.Empty;
		}

		public IEnumerable<TextTag> Highlight(TextTag text)
		{
			_targetTag = text;

			var pattern = BuildPattern();
			if(string.IsNullOrWhiteSpace(pattern))
				return new [] { text };

			var splits = Regex.Split(text.Text, pattern);

			return CreateTags(splits, Matches, Modify);
		}

		public IEnumerable<TextTag> CreateTags(string[] split, Func<string, bool> matches, Action<TextTag> modify)
		{
			var tags = new List<TextTag>();

			split.Apply(s => {
				if(!string.IsNullOrWhiteSpace(s))
				{
					var tag = TextTag.For(s, _targetTag);
					if(matches(s))
						modify(tag);
					tags.Add(tag);
				}
			});

			return tags;
		}
	}
}
