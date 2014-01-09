using System;
using System.Xml.Linq;

namespace Outlander.Core.Text
{
	public class PromptChunkReader : IChunkReader
	{
		private readonly ChunkReader<PromptTag> _reader;
		private int _count;

		private PromptTag _lastPrompt;

		public PromptChunkReader()
		{
			_reader = new ChunkReader<PromptTag>("<prompt", "</prompt", skipNewLineAfterTag: false);
			_reader.Append = (builder, result, tag) => {

				var skip = 0;

				if(_count == 0 && !tag.Equals(_lastPrompt)) {
					builder.Append(tag.Prompt);
				}
				else {
					skip = 1;
				}

				_lastPrompt = tag;
				result.AddTag(tag);
				_count++;
				return skip;
			};
		}

		public ReadResult Read(Chunk chunk)
		{
			var result = _reader.Read(chunk);
			_count = 0;
			return result;
		}
	}
}
