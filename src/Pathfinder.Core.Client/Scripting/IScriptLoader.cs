using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Pathfinder.Core.Client.Scripting
{
	public interface IScriptLoader
	{
		bool CanLoad(string name);
		string Load(string name);
	}

	public class ScriptLoader : IScriptLoader
	{
		private const string ScriptFileType = ".cmd";

		private readonly IFileSystem _fileSystem;
		private readonly AppSettings _settings;

		public ScriptLoader(IFileSystem fileSystem, AppSettings settings)
		{
			_fileSystem = fileSystem;
			_settings = settings;
		}

		public bool CanLoad(string name)
		{
			return _fileSystem.Exists(ScriptPathFor(name));
		}

		public string Load(string name)
		{
			var path = ScriptPathFor(name);
			return _fileSystem.ReadAllText(path);
		}

		public string ScriptPathFor(string name)
		{
			return Path.Combine(_settings.HomeDirectory, AppSettings.ScriptsFolder, name + ScriptFileType);
		}
	}
}
