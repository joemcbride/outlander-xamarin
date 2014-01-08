using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Client
{
	public class Highlights
	{
		private IEnumerable<IHighlighter> _highlighters;

		public Highlights(IEnumerable<IHighlighter> highlighters)
		{
			_highlighters = highlighters;
		}

		public IEnumerable<TextTag> For(TextTag text)
		{
			var results = new List<TextTag>();
			results.Add(text);

			_highlighters.Apply(h => {

				for (int i = 0; i < results.Count; i++) {
					var tag = results[i];

					results.RemoveAt(i);
					var tags = h.Highlight(tag);
					results.InsertRange(i, tags);
					i+= tags.Count() - 1;
				}
			});

			results.Apply(t => {
				t.Text = Regex.Replace(t.Text, "<[^>]*>", string.Empty);
			});

			return results;
		}
	}
}
