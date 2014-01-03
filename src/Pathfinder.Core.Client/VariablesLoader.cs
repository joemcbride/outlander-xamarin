using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.IO;

namespace Pathfinder.Core.Client
{
	public interface IVariablesLoader
	{
		void Save(IDictionary<string, string> vars, string fileName);
		IEnumerable<Variable> Load(string path);
		IEnumerable<Variable> Parse(string vars);
	}

	public class VariablesLoader : IVariablesLoader
	{
		private readonly IFileSystem _fileSystem;
		private readonly AppSettings _settings;

		public VariablesLoader(IFileSystem fileSystem, AppSettings settings)
		{
			_fileSystem = fileSystem;
			_settings = settings;
		}

		public void Save(IDictionary<string, string> vars, string fileName)
		{
			var path = Path.Combine(_settings.HomeDirectory, fileName);
			var builder = new StringBuilder();

			vars.Apply(x => builder.AppendLine("#var {{{0}}} {{{1}}}".ToFormat(x.Key, x.Value)));

			_fileSystem.Save(builder.ToString(), path);
		}

		public IEnumerable<Variable> Load(string path)
		{
			var vars = _fileSystem.ReadAllText(path);
			return Parse(vars);
		}

		public IEnumerable<Variable> Parse(string vars)
		{
			return Regex
					.Matches(vars, "^#var {(.*)} {(.*)}$", RegexOptions.Multiline)
					.OfType<Match>()
					.Select(x => new Variable { Key = x.Groups[1].Value, Value = x.Groups[2].Value })
					.ToList();
		}
	}

	public class Variable
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}
}
