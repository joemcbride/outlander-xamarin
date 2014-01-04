using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading;

namespace Pathfinder.Core.Client
{
	public interface IHighlightsLoader
	{
		IEnumerable<Highlight> Load(string path);
		IEnumerable<Highlight> Parse(string highlights);
	}

	public class HighlightsLoader : IHighlightsLoader
	{
		private static ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

		private readonly IFileSystem _fileSystem;

		public HighlightsLoader(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
		}

		public void Save()
		{
			Lock.Write(() => {
			});
		}

		public IEnumerable<Highlight> Load(string path)
		{
			return Lock.Read(() => {
				var data = _fileSystem.ReadAllText(path);
				return Parse(data);
			});
		}

		public IEnumerable<Highlight> Parse(string highlights)
		{
			return Regex
					.Matches(highlights, "^#highlight {(.*)} {(.*)}$", RegexOptions.Multiline)
					.OfType<Match>()
					.OrderByDescending(x => x.Value.Length)
					.Select(x => new Highlight { Color = x.Groups[1].Value, Pattern = x.Groups[2].Value });
		}
	}

	public class Highlight
	{
		public string Pattern { get; set; }
		public string Color { get; set; }
		public bool Mono { get; set; }
	}
}
