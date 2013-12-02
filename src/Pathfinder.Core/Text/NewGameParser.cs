using System;
using System.Collections.Generic;

namespace Pathfinder.Core.Text
{
	public class NewGameParser
	{
		private List<IChunkReader> _parsers;

		public NewGameParser()
		{
			_parsers = new List<IChunkReader>();
			_parsers.Add(new ChunkReader<Tag>("<pushStream", "<popStream", true));
			_parsers.Add(new ChunkReader<RoomNameTag>("<style id=\"roomName\"", "<style id=\"\"", true));
			_parsers.Add(new ChunkReader<PromptTag>("<prompt", "</prompt"));
			_parsers.Add(new ChunkReader<Tag>("<component", "</component"));
			_parsers.Add(new ChunkReader<Tag>("<openDialog", "</openDialog"));
			_parsers.Add(new ChunkReader<VitalsTag>("<dialogData", "</dialogData"));
			_parsers.Add(new ChunkReader<Tag>("<compass", "</compass"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<app"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<exposeContainer"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<inv"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<container"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<nav"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<roundTime"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<castTime"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<streamWindow"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<clearStream"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<mode"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<switchQuickBar"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<indicator"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<resource"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<output"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<endSetup"));
			_parsers.Add(new ChunkReader<Tag>("<spell", "</spell"));
			_parsers.Add(new ChunkReader<Tag>("<left", "</left"));
			_parsers.Add(new ChunkReader<Tag>("<right", "</right"));
		}

		private IChunkReader _lastReader;

		public ReadResult Parse(Chunk chunk)
		{
			var overallResult = new ReadResult();

			foreach (var parser in _parsers) {

				if (chunk == null)
					break;

				var result = parser.Read(chunk);
				chunk = result.Chunk;
				overallResult.AddTags(result.Tags);
			}

			if(chunk != null && !string.IsNullOrWhiteSpace(chunk.Text))
			{
				overallResult.Chunk = chunk;
			}

			return overallResult;
		}
	}
}
