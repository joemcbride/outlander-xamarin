using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Pathfinder.Core.Client.Scripting;

namespace Pathfinder.Core.Client
{
	public class TokenDefinitionRegistry
	{
		private List<TokenDefinition> _definitions = new List<TokenDefinition>();

		public void Add(TokenDefinition token)
		{
			_definitions.Add(token);
		}

		public IEnumerable<TokenDefinition> Definitions()
		{
			return _definitions;
		}

		public void New(Action<TokenDefinition> configure)
		{
			var def = new TokenDefinition();
			configure(def);
			_definitions.Add(def);
		}

		public static TokenDefinitionRegistry ClientCommands()
		{
			var registry = new TokenDefinitionRegistry();

			registry.New(d => {
				d.Type = "script";
				d.Pattern = "^\\.([a-zA-Z0-9]+)";
				d.Ignore = false;
				d.BuildToken =  (source, match, def)=> {

					var value = source.Substring(match.Groups[1].Index, source.Length - match.Groups[1].Index);
					var args = source.Substring(match.Groups[1].Index + match.Groups[1].Length, source.Length - (match.Groups[1].Index + match.Groups[1].Length));

					var splitArgs = Regex
						.Matches(args, @"(?<match>\w+)|\""(?<match>[\w\s]*)""")
						.Cast<Match>()
						.Select(m => m.Groups["match"].Value)
						.ToArray();

					var token = new ScriptToken
					{
						Name = match.Groups[1].Value,
						Text = source,
						Type = def.Type,
						Value = value,
						Args = splitArgs
					};
					return token;
				};
			});

			registry.New(d => {
				d.Type = "scriptcommand";
				d.Pattern = "^#script";
				d.Ignore = false;
				d.BuildToken =  (source, match, def)=> {
					var token = new Token
					{
						Text = source,
						Type = def.Type,
						Value = source.Substring(match.Index + match.Length, source.Length - (match.Index + match.Length)).Trim()
					};
					return token;
				};
			});

			return registry;
		}

		public static TokenDefinitionRegistry Default()
		{
			var registry = new TokenDefinitionRegistry();

			registry.New(d => {
				d.Type = "comment";
				d.Pattern = "^#.*";
				d.Ignore = true;
				d.BuildToken =  (source, match, def)=> {
					var token = new Token
					{
						Text = source,
						Type = def.Type,
						Value = match.Value
					};
					return token;
				};
			});

			registry.New(d => {
				d.Type = "label";
				d.Pattern = "^([a-zA-Z0-9]+):";
				d.Ignore = false;
				d.BuildToken =  (source, match, def)=> {
					var token = new Token
					{
						Text = source,
						Type = def.Type,
						Value = match.Groups[1].Value
					};
					return token;
				};
			});

			registry.New(d => {
				d.Type = "goto";
				d.Pattern = "^goto";
				d.Ignore = false;
				d.BuildToken =  (source, match, def)=> {
					var token = new Token
					{
						Text = source,
						Type = def.Type,
						Value = source.Substring(match.Index + match.Length, source.Length - (match.Index + match.Length)).Trim()
					};
					return token;
				};
			});

			registry.New(d => {
				d.Type = "waitfor";
				d.Pattern = "^waitfor";
				d.Ignore = false;
				d.BuildToken =  (source, match, def)=> {
					var token = new Token
					{
						Text = source,
						Type = def.Type,
						Value = source.Substring(match.Index + match.Length, source.Length - (match.Index + match.Length)).Trim()
					};
					return token;
				};
			});

			registry.New(d => {
				d.Type = "match";
				d.Pattern = "^match\\b";
				d.Ignore = false;
				d.BuildToken =  (source, match, def)=> {
					const string Goto_Regex = "^match\\b\\s([a-zA-z0-9\\.]+)\\s(.*)";
					var gotoMatch = Regex.Match(source, Goto_Regex);
					var token = new MatchToken
					{
						Text = source,
						Type = def.Type,
						IsRegex = false,
						Value = source.Substring(match.Index + match.Length, source.Length - (match.Index + match.Length)).Trim(),
						Goto = gotoMatch.Groups[1].Value,
						Pattern = gotoMatch.Groups[2].Value
					};
					return token;
				};
			});

			registry.New(d => {
				d.Type = "matchre";
				d.Pattern = "^matchre\\b";
				d.Ignore = false;
				d.BuildToken =  (source, match, def)=> {
					const string Goto_Regex = "^matchre\\b\\s([a-zA-z0-9\\.]+)\\s(.*)";
					var gotoMatch = Regex.Match(source, Goto_Regex);
					var token = new MatchToken
					{
						Text = source,
						Type = def.Type,
						IsRegex = true,
						Value = source.Substring(match.Index + match.Length, source.Length - (match.Index + match.Length)).Trim(),
						Goto = gotoMatch.Groups[1].Value,
						Pattern = gotoMatch.Groups[2].Value
					};
					return token;
				};
			});

			registry.New(d => {
				d.Type = "matchwait";
				d.Pattern = "^matchwait";
				d.Ignore = false;
				d.BuildToken =  (source, match, def)=> {
					var token = new Token
					{
						Text = source,
						Type = def.Type,
						Value = source.Substring(match.Index + match.Length, source.Length - (match.Index + match.Length)).Trim()
					};
					return token;
				};
			});

			registry.New(d => {
				d.Type = "pause";
				d.Pattern = "^pause";
				d.Ignore = false;
				d.BuildToken =  (source, match, def)=> {
					var token = new Token
					{
						Text = source,
						Type = def.Type,
						Value = source.Substring(match.Index + match.Length, source.Length - (match.Index + match.Length)).Trim()
					};
					return token;
				};
			});

			registry.New(d => {
				d.Type = "put";
				d.Pattern = "^put";
				d.Ignore = false;
				d.BuildToken =  (source, match, def)=> {
					var token = new Token
					{
						Text = source,
						Type = def.Type,
						Value = source.Substring(match.Index + match.Length, source.Length - (match.Index + match.Length)).Trim()
					};
					return token;
				};
			});

			return registry;
		}
	}
}
