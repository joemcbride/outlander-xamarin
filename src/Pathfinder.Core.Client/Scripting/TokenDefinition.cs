using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Outlander.Core.Client
{
	public class TokenDefinition
	{
		public string Type { get; set; }
		public string Pattern { get; set; }
		public bool Ignore { get; set; }
		public Func<string, Match, TokenDefinition, Token> BuildToken { get; set; }
	}
}
