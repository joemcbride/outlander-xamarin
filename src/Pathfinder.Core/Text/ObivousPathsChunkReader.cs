using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text
{
	public class YouAlsoSeeChunkReader : IChunkReader
	{
		const string Paths_Regex = "</preset>  You also see";

		public ReadResult Read(Chunk chunk)
		{
			var result = new ReadResult();

			if(chunk.Text != null && Regex.IsMatch(chunk.Text, Paths_Regex))
			{
				var replaced = Regex.Replace(chunk.Text, Paths_Regex, "</preset>  \nYou also see");

				result.Chunk = Chunk.For(replaced);
			}
			else
			{
				result.Chunk = chunk;
			}

			return result;
		}
	}

	public class ObivousPathsChunkReader : IChunkReader
	{
		const string Paths_Regex = "([\n]*)Obvious (paths|exits): (.*)\\.";

		public ReadResult Read(Chunk chunk)
		{
			var result = new ReadResult();

			var match = Regex.Match(chunk.Text, Paths_Regex);

			if(match.Success)
			{
				// remove extra line feeds
				var newLines = match.Groups[1].Value.Length > 0 ? "\n" : string.Empty;
				var replaced =
					chunk.Text.Replace(
						match.Groups[0].Value,
						"{0}Obvious {1}: {2}.".ToFormat(
							newLines,
							match.Groups[2].Value,
							match.Groups[3].Value));

				result.Chunk = Chunk.For(replaced);
			}
			else
			{
				result.Chunk = chunk;
			}

			return result;
		}
	}
}
