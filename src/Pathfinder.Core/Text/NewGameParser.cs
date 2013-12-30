using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Pathfinder.Core.Text
{
	public interface IGameParser
	{
		ReadResult Parse(Chunk chunk);
	}

	public class NewGameParser : IGameParser
	{
		private readonly List<IChunkReader> _parsers;
		private readonly IEnumerable<ITagTransformer> _tagTransformers;

		public NewGameParser(IEnumerable<ITagTransformer> tagTransformers)
		{
			_tagTransformers = tagTransformers;

			_parsers = new List<IChunkReader>();
			_parsers.Add(new StreamChunkReader());
			_parsers.Add(new RoomNameChunkReader());
			_parsers.Add(new PromptChunkReader());
			// component needs to go before preset
			_parsers.Add(new ChunkReader<ComponentTag>("<component", "</component"));
			_parsers.Add(new PresetChunkReader());
			_parsers.Add(new ChunkReader<Tag>("<openDialog", "</openDialog"));
			_parsers.Add(new ChunkReader<VitalsTag>("<dialogData", "</dialogData"));
			_parsers.Add(new ChunkReader<Tag>("<compass", "</compass"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<clearContainer"));
			_parsers.Add(new SelfClosingChunkReader<AppTag>("<app"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<exposeContainer"));
			_parsers.Add(new ChunkReader<Tag>("<inv", "</inv"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<container"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<nav"));
			_parsers.Add(new SelfClosingChunkReader<RoundtimeTag>("<roundTime"));
			_parsers.Add(new SelfClosingChunkReader<CasttimeTag>("<castTime"));
			_parsers.Add(new SelfClosingChunkReader<StreamWindowTag>("<streamWindow"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<clearStream"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<mode"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<switchQuickBar"));
			_parsers.Add(new SelfClosingChunkReader<IndicatorTag>("<indicator"));
			_parsers.Add(new SelfClosingChunkReader<Tag>("<resource"));
			_parsers.Add(new OutputChunkReader());
			_parsers.Add(new SelfClosingChunkReader<Tag>("<endSetup"));
			_parsers.Add(new ChunkReader<SpellTag>("<spell", "</spell"));
			_parsers.Add(new ChunkReader<LeftHandTag>("<left", "</left"));
			_parsers.Add(new ChunkReader<RightHandTag>("<right", "</right"));
			_parsers.Add(new ObivousPathsChunkReader());
		}

		private int _lastIdx = 0;

		public ReadResult Parse(Chunk chunk)
		{
			var overallResult = new ReadResult();

			System.Diagnostics.Debug.WriteLine("Parsing: " + chunk.Text);

			if(_lastIdx > -1)
			{
				var parser = _parsers[_lastIdx];
				var result = parser.Read(chunk);
				chunk = result.Chunk;
				overallResult.AddTags(result.Tags);
				if (result.Stop) {
					return overallResult;
				}

				_lastIdx = -1;
			}

			for (var i = 0; i < _parsers.Count; i++) {
				if (chunk == null)
					break;

				var parser = _parsers[i];

				var result = parser.Read(chunk);
				chunk = result.Chunk;
				overallResult.AddTags(result.Tags);

				if (result.Stop) {
					_lastIdx = i;
				}
			}

			if(chunk != null && !string.IsNullOrWhiteSpace(chunk.Text))
			{
				overallResult.Chunk = chunk;
			}

			var tags = Transform(overallResult.Tags);
			overallResult.ClearTags();
			overallResult.AddTags(tags);

			return overallResult;
		}

		private IEnumerable<Tag> Transform(IEnumerable<Tag> tags)
		{
			var newTags = new List<Tag>();
			tags.Apply(tag => {
				_tagTransformers.Apply(t =>
					{
						if(t.Matches(tag)) tag = t.Transform(tag);
					});
				newTags.Add(tag);
			});

			return newTags;
		}
	}
}
