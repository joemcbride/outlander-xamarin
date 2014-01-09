using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Outlander.Core.Client
{
	public class Tokenizer
	{
		private List<TokenDefinition> _definitions = new List<TokenDefinition>();

		public Tokenizer(IEnumerable<TokenDefinition> definitions)
		{
			_definitions.AddRange(definitions);
		}

		public IEnumerable<Token> Tokenize(string source)
		{
			var tokens = new List<Token>();

			foreach(var item in _definitions) {
				var match = Regex.Match(source, item.Pattern, RegexOptions.IgnoreCase);
				if(match.Success) {
					var token = item.BuildToken(source, match, item);

					if(token != null) {
						tokens.Add(token);
					}
				}
			}

			return tokens;
		}

		public static Tokenizer With(TokenDefinitionRegistry registry)
		{
			return new Tokenizer(registry.Definitions());
		}
	}
}
