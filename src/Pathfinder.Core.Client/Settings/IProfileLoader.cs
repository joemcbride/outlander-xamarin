using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;

namespace Outlander.Core.Client
{
	public interface IProfileLoader
	{
		IEnumerable<Profile> Profiles();
		Profile Load(string path);
		void Save(Profile profile);
		void Remove(string profile);
		bool Exists(string profile);
	}

	public class ProfileLoader : IProfileLoader
	{
		private const string Config_File_Name = "config.cfg";

		private readonly IFileSystem _fileSystem;
		private readonly IDirectorySystem _directorySystem;
		private readonly IAppDirectoriesBuilder _appDirBuilder;
		private readonly AppSettings _settings;

		private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

		public ProfileLoader(IFileSystem fileSystem, IDirectorySystem directorySystem, IAppDirectoriesBuilder appDirBuilder, AppSettings settings)
		{
			_fileSystem = fileSystem;
			_directorySystem = directorySystem;
			_appDirBuilder = appDirBuilder;
			_settings = settings;
		}

		public IEnumerable<Profile> Profiles()
		{
			var profilePath = PathForProfiles();
			var dirs = _directorySystem.Directories(profilePath);

			return dirs.Select(dir => Load(new DirectoryInfo(dir).Name)).ToList();
		}

		public Profile Load(string name)
		{
			return Lock.Read(() => {
				var filePath = Path.Combine(PathForProfiles(), name, Config_File_Name);

				if(!_fileSystem.Exists(filePath))
					return new Profile { Name = name };

				var text = _fileSystem.ReadAllText(filePath);

				var accountMatch = Regex.Match(text, "Account: (?<account>\\S*)", RegexOptions.IgnoreCase);
				var gameMatch = Regex.Match(text, "Game: (?<game>\\S*)", RegexOptions.IgnoreCase);
				var characterMatch = Regex.Match(text, "Character: (?<character>\\S*)", RegexOptions.IgnoreCase);

				var profile = new Profile
				{
					Name = new DirectoryInfo(name).Name,
					Account = accountMatch.Success ? accountMatch.Groups["account"].Value.Trim() : string.Empty,
					Game = gameMatch.Success ? gameMatch.Groups["game"].Value.Trim() : string.Empty,
					Character = characterMatch.Success ? characterMatch.Groups["character"].Value.Trim() : string.Empty
				};

				return profile;
			});
		}

		public void Save(Profile profile)
		{
			Lock.Write(() => {
				var filePath = Path.Combine(PathForProfiles(), profile.Name, Config_File_Name);

				_appDirBuilder.BuildProfile(profile.Name);

				var builder = new StringBuilder();
				builder.AppendFormat("Account: {0}\n", profile.Account);
				builder.AppendFormat("Game: {0}\n", profile.Game);
				builder.AppendFormat("Character: {0}", profile.Character);

				_fileSystem.Save(builder.ToString(), filePath);
			});
		}

		public void Remove(string profile)
		{
			Lock.Write(() => {
				var filePath = Path.Combine(PathForProfiles(), profile);
				_directorySystem.Delete(filePath);
			});
		}

		public bool Exists(string profile)
		{
			return Lock.Read(() => {
				var filePath = Path.Combine(PathForProfiles(), profile);
				return _directorySystem.Exists(filePath);
			});
		}

		private string PathForProfiles()
		{
			return Path.Combine(
				_settings.HomeDirectory,
				AppSettings.ConfigFolder,
				AppSettings.ProfilesFolder);
		}
	}
}
