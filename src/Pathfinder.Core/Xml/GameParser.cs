using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinder.Core
{
	public interface ITransformer
	{
		bool Matches(string data);
		string Transform(string data);
	}

	public interface IParser
	{
		bool Matches(string data);
		IEnumerable<ParseResult> Parse(string data);
	}

	public class GameParser
	{
		private IEnumerable<ITransformer> _transformers;
		private IEnumerable<IParser> _parsers;

		public GameParser(IEnumerable<ITransformer> transformers, IEnumerable<IParser> parsers)
		{
			_transformers = transformers;
			_parsers = parsers;
		}

		public IEnumerable<ParseResult> Parse(GameData data)
		{
			var results = new List<ParseResult>();

			_transformers.Apply(t => {
				if(t.Matches(data.Current))
				{
					var result = t.Transform(data.Current);
					data.Replace(result);
				}
			});

			_parsers.Apply(
				p => {
					if (p.Matches(data.Current))
					{
						var parseResults = p.Parse(data.Current);
						parseResults.Apply(r => data.Remove(r.Matched));
						results.AddRange(parseResults);
					}
				});

			return results;
		}
	}

	public class GameData
	{
		private StringBuilder _data = new StringBuilder();

		public string Current
		{
			get
			{
				return _data.ToString();
			}
		}

		public void Append(string text)
		{
			_data.Append(text);
		}

		public void Remove(string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				_data.Replace(text, string.Empty);
			}
		}

		public void Replace(string text)
		{
			_data.Clear();
			_data.Append(text);
		}

		public void Clear()
		{
			_data.Clear();
		}
	}
}
