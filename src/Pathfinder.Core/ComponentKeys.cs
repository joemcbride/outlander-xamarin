using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;

namespace Pathfinder.Core
{
	public static class ComponentKeys
	{
		public const string Prompt = "prompt";

		public const string CharacterName = "charactername";
		public const string Game = "game";

		public const string Health = "health";
		public const string Mana = "mana";
		public const string Stamina = "stamina";
		public const string Concentration = "concentration";
		public const string Spirit = "spirit";

		public const string RoomTitle = "roomtitle";
		public const string RoomName = "roomname";
		public const string RoomDescription = "roomdesc";
		public const string RoomExists = "roomexits";
		public const string RoomObjects = "roomobjs";
		public const string RoomPlayers = "roomplayers";

		public const string Spell = "spell";
		public const string GameTime = "gametime";
		public const string GameTimeUpdate = "gametimeupdate";

		public const string LeftHand = "lefthand";
		public const string LeftHandId = "lefthandnounid";
		public const string LeftHandNoun = "lefthandnoun";

		public const string RightHand = "righthand";
		public const string RightHandId = "righthandnounid";
		public const string RightHandNoun = "righthandnoun";

		public const string Roundtime = "roundtime";
	}
}
