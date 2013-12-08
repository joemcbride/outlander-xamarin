using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text
{

	public class ObivousPathsChunkReader : IChunkReader
	{
		const string Paths_Regex = "[\r\n]*Obvious (paths|exits): (.*)\\.";

		public ReadResult Read(Chunk chunk)
		{
			var result = new ReadResult();

			if(chunk.Text != null)
			{
				try {
					var replaced = Regex.Replace(chunk.Text, Paths_Regex, "\nObvious $1: $2.");
					result.Chunk = Chunk.For(replaced);
				} catch(Exception exc){
					Console.WriteLine(exc);
					result.Chunk = chunk;
				}
			}

			return result;
		}
	}
	
}
