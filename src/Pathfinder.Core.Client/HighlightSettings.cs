using System;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinder.Core.Client
{
	public class HighlightSettings
	{
		private IList<HighlightSetting> _settings = new List<HighlightSetting>();

		public void Add(HighlightSetting setting)
		{
			_settings.Add(setting);
		}

		public HighlightSetting Get(string id)
		{
			return _settings.FirstOrDefault(x => x.Id == id);
		}

		public static HighlightSettings Default()
		{
			var settings = new HighlightSettings();

			settings.Add(new HighlightSetting { Id = HighlightKeys.Default, Color = "#F5F5F5"  });
			settings.Add(new HighlightSetting { Id = HighlightKeys.Bold, Color = "#FFFF00"  });
			settings.Add(new HighlightSetting { Id = HighlightKeys.RoomName, Color = "#0000FF"  });
			settings.Add(new HighlightSetting { Id = HighlightKeys.Whisper, Color = "#00FFFF"  });

			return settings;
		}
	}

	public static class HighlightKeys
	{
		public const string Default = "default";
		public const string Bold = "bold";
		public const string RoomName = "roomname";
		public const string Whisper = "whisper";
	}

	public class HighlightSetting
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Color { get; set; }
		public bool Mono { get; set; }
	}
}
