using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Pathfinder.Core.Client
{
	public class HighlightLoader
	{
		private HighlightSettings _settings;

		public HighlightLoader(HighlightSettings settings)
		{
			_settings = settings;
		}

		public void Save()
		{}

		public void Load(string highlights)
		{
		}

		public IEnumerable<IHighlighter> Parse(string highlights)
		{
			return Regex
				.Matches(highlights, "")
				.OfType<Match>()
					.Select(x => new SimpleHighlighter("", Guid.NewGuid().ToString(), _settings));
		}
	}
}
