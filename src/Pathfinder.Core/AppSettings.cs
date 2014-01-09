using System;

namespace Outlander.Core
{
	public class AppSettings
	{
		public const string ScriptsFolder = "Scripts";
		public const string LogFolder = "Logs";
		public const string ConfigFolder = "Config";
		public const string ProfilesFolder = "Profiles";

		public const string VariablesFileName = "variables.cfg";
		public const string HighlightsFileName = "highlights.cfg";

		public string HomeDirectory { get; set; }
	}
}
