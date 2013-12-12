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

		public const string RoomTitle = "roomtitle";
		public const string RoomName = "roomname";
		public const string RoomDescription = "roomdesc";
		public const string RoomExists = "roomexits";
		public const string RoomObjects = "roomobjs";
		public const string RoomPlayers = "roomplayers";

		public const string Spell = "spell";
		public const string GameTime = "gametime";

		public const string LeftHand = "lefthand";
		public const string LeftHandId = "lefthandnounid";
		public const string LeftHandNoun = "lefthandnoun";

		public const string RightHand = "righthand";
		public const string RightHandId = "righthandnounid";
		public const string RightHandNoun = "righthandnoun";

		public const string Roundtime = "roundtime";
	}
}
