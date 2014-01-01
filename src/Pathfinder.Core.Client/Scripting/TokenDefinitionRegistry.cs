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
				d.Pattern = "^\\.(\\w+)";
				d.Ignore = false;
				d.BuildToken = (source, match, def)=> {

					var value = source.Substring(match.Groups[1].Index, source.Length - match.Groups[1].Index);
					var args = source.Substring(match.Groups[1].Index + match.Groups[1].Length, source.Length - (match.Groups[1].Index + match.Groups[1].Length));

					var splitArgs = Regex
						.Matches(args, @"(?<match>\w+)|\""(?<match>[\w\s]*)""")
						.Cast<Match>()
						.Select(m => m.Groups["match"].Value)
						.ToArray();

					var token = new ScriptToken
					{
						Id = Guid.NewGuid().ToString(),
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
				d.BuildToken = (source, match, def)=> {
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
				d.BuildToken = (source, match, def)=> {
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
				d.Pattern = RegexPatterns.Label;
				d.Ignore = false;
				d.BuildToken = (source, match, def)=> {
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
				d.BuildToken = (source, match, def)=> {
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
				d.Pattern = "^waitfor\\b";
				d.Ignore = false;
				d.BuildToken = (source, match, def)=> {
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
				d.Type = "waitforre";
				d.Pattern = "^waitforre\\b";
				d.Ignore = false;
				d.BuildToken = (source, match, def)=> {
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
				d.BuildToken = (source, match, def)=> {
					const string Goto_Regex = "^match\\b\\s([\\w\\.]+)\\s(.*)";
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
				d.BuildToken = (source, match, def)=> {
					const string Goto_Regex = "^matchre\\b\\s([\\w\\.]+)\\s(.*)";
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
				d.BuildToken = (source, match, def)=> {
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
				d.BuildToken = (source, match, def)=> {
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
				d.BuildToken = (source, match, def)=> {
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
				d.Type = "echo";
				d.Pattern = "^echo";
				d.Ignore = false;
				d.BuildToken = (source, match, def)=> {
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
				d.Type = "var";
				d.Pattern = "^var";
				d.Ignore = false;
				d.BuildToken = (source, match, def)=> {
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
				d.Type = "var";
				d.Pattern = "^setvariable";
				d.Ignore = false;
				d.BuildToken = (source, match, def)=> {
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
				d.Type = "unvar";
				d.Pattern = "^unvar\\b";
				d.Ignore = false;
				d.BuildToken = (source, match, def)=> {
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
				d.Type = "hasvar";
				d.Pattern = "^hasvar\\b";
				d.Ignore = false;
				d.BuildToken = (source, match, def)=> {
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
				d.Type = "save";
				d.Pattern = "^save";
				d.Ignore = false;
				d.BuildToken = (source, match, def)=> {
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
				d.Type = "if";
				d.Pattern = "^if\\b";
				d.Ignore = false;
				d.BuildToken = (source, match, def)=> {

					var value = source.Substring(match.Index + match.Length, source.Length - (match.Index + match.Length)).Trim();

					var token = new IfToken
					{
						Text = source,
						Type = def.Type,
						Value = value
					};
					return token;
				};
			});

			registry.New(d => {
				d.Type = "if_";
				d.Pattern = "^if_(\\d+)";
				d.Ignore = false;
				d.BuildToken = (source, match, def)=> {

					var value = source.Substring(match.Index + match.Length, source.Length - (match.Index + match.Length)).Trim();

					var token = new IfToken
					{
						Text = source,
						Type = def.Type,
						Value = match.Value,
						ReplaceBlocks = false,
						Blocks = new IfBlocks
						{
							IfEval = match.Groups[1].Value,
							IfBlock = value
						}
					};
					return token;
				};
			});

			registry.New(d => {
				d.Type = "move";
				d.Pattern = "^move";
				d.Ignore = false;
				d.BuildToken = (source, match, def)=> {

					var value = source.Substring(match.Index + match.Length, source.Length - (match.Index + match.Length)).Trim();

					var token = new Token
					{
						Text = source,
						Type = def.Type,
						Value = value
					};
					return token;
				};
			});

			registry.New(d => {
				d.Type = "nextroom";
				d.Pattern = "^nextroom";
				d.Ignore = false;
				d.BuildToken = (source, match, def)=> {

					var value = source.Substring(match.Index + match.Length, source.Length - (match.Index + match.Length)).Trim();

					var token = new Token
					{
						Text = source,
						Type = def.Type,
						Value = value
					};
					return token;
				};
			});

			registry.New(d => {
				d.Type = "action";
				d.Pattern = "^action\\b(.*)\\bwhen\\b(.*)";
				d.Ignore = false;
				d.BuildToken = (source, match, def)=> {

					var value = source.Substring(match.Index + match.Length, source.Length - (match.Index + match.Length)).Trim();

					var token = new ActionToken
					{
						Text = source,
						Type = def.Type,
						Value = value,
						Action = match.Groups[1].Value.Trim(),
						When = match.Groups[2].Value.Trim()
					};
					return token;
				};
			});

			return registry;
		}
	}

	public class IfToken : Token
	{
		public IfToken()
		{
			ReplaceBlocks = true;
		}

		public bool ReplaceBlocks { get; set; }
		public IfBlocks Blocks { get; set; } 
	}
}
