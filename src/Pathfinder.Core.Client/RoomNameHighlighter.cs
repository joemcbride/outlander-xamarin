using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Client
{
	public class RoomNameHighlighter : RegexHighlighter
	{
		private IGameState _gameState;
		private HighlightSettings _settings;

		public RoomNameHighlighter(IGameState gameState, HighlightSettings settings)
		{
			_gameState = gameState;
			_settings = settings;

			Matches = (s) => {
				return s == _gameState.Get(ComponentKeys.RoomName) || s == _gameState.Get(ComponentKeys.RoomTitle);
			};
			Modify = (tag) => {
				var setting = _settings.Get(HighlightKeys.RoomName);
				tag.Color = setting.Color;
				tag.Mono = setting.Mono;
				tag.Matched = true;
			};
		}

		public override string BuildPattern()
		{
			var originalRoomName = _gameState.Get(ComponentKeys.RoomName);
			if(string.IsNullOrWhiteSpace(originalRoomName))
				return string.Empty;

			var originalRoomTitle = _gameState.Get(ComponentKeys.RoomTitle);

			var roomName = "(" + FilterName(originalRoomName) + "|" + FilterName(originalRoomTitle) + ")";
			return roomName;

			//return "(\\[.*?\\])";
		}

		private string FilterName(string name)
		{
			return name.Replace("[", "\\[").Replace("]", "\\]").Replace(",", "\\,");
		}
	}
}
