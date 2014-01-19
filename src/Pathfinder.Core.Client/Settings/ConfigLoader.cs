using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;

namespace Outlander.Core.Client
{
	public interface IConfigLoader
	{
		void Load(string filename);
		void Save(string filename);
	}

	public class ConfigLoader : IConfigLoader
	{
		private readonly IFileSystem _fileSystem;
		private readonly IDirectorySystem _directorySystem;
		private readonly AppSettings _settings;

		private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

		public ConfigLoader(IFileSystem fileSystem, IDirectorySystem directorySystem, AppSettings settings)
		{
			_fileSystem = fileSystem;
			_directorySystem = directorySystem;
			_settings = settings;
		}

		public void Load(string filename)
		{
			Lock.Read(() => {
				if(_fileSystem.Exists(filename)) {
					var text = _fileSystem.ReadAllText(filename);
					var match = Regex.Match(text, "Profile: (?<profile>\\S*)");
					var profile = match.Success ? match.Groups["profile"].Value : AppSettings.DefaultProfile;
					_settings.Profile = !string.IsNullOrWhiteSpace(profile) ? profile.Trim() : AppSettings.DefaultProfile;
				}
			});
		}

		public void Save(string filename)
		{
			Lock.Write(() => {
				var builder = new StringBuilder();
				builder.AppendFormat("Profile: {0}\n", _settings.Profile);
				_fileSystem.Save(builder.ToString(), filename);
			});
		}
	}
}
