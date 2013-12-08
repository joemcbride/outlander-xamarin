using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Pathfinder.Core
{
	public class LearningRate
	{
		public static LearningRate Clear = new LearningRate(0, "clear");
		public static LearningRate Dabbling = new LearningRate(1, "dabbling");
		public static LearningRate Perusing = new LearningRate(2, "perusing");
		public static LearningRate Learning = new LearningRate(3, "learning");
		public static LearningRate Thoughtful = new LearningRate(4, "thoughtful");
		public static LearningRate Thinking = new LearningRate(5, "thinking");
		public static LearningRate Considering = new LearningRate(6, "considering");
		public static LearningRate Pondering = new LearningRate(7, "pondering");
		public static LearningRate Ruminating = new LearningRate(8, "ruminating");
		public static LearningRate Concentrating = new LearningRate(9, "concentrating");
		public static LearningRate Attentive = new LearningRate(10, "attentive");
		public static LearningRate Deliberative = new LearningRate(11, "deliberative");
		public static LearningRate Interested = new LearningRate(12, "interested");
		public static LearningRate Examining = new LearningRate(13, "examining");
		public static LearningRate Understanding = new LearningRate(14, "understanding");
		public static LearningRate Absorbing = new LearningRate(15, "absorbing");
		public static LearningRate Intrigued = new LearningRate(16, "intrigued");
		public static LearningRate Scrutinizing = new LearningRate(17, "scrutinizing");
		public static LearningRate Analyzing = new LearningRate(18, "analyzing");
		public static LearningRate Studious = new LearningRate(19, "studious");
		public static LearningRate Focused = new LearningRate(20, "focused");
		public static LearningRate VeryFocused = new LearningRate(21, "very focused");
		public static LearningRate Engaged = new LearningRate(22, "engaged");
		public static LearningRate VeryEngaged = new LearningRate(23, "very engaged");
		public static LearningRate Cogitating = new LearningRate(24, "cogitating");
		public static LearningRate Fascinated = new LearningRate(25, "fascinated");
		public static LearningRate Captivated = new LearningRate(26, "captivated");
		public static LearningRate Engrossed = new LearningRate(27, "engrossed");
		public static LearningRate Riveted = new LearningRate(28, "riveted");
		public static LearningRate VeryRiveted = new LearningRate(29, "very riveted");
		public static LearningRate Rapt = new LearningRate(30, "rapt");
		public static LearningRate VeryRapt = new LearningRate(31, "very rapt");
		public static LearningRate Enthralled = new LearningRate(32, "enthralled");
		public static LearningRate NearlyLocked = new LearningRate(33, "nearly locked");
		public static LearningRate MindLocked = new LearningRate(34, "mind locked");

		public LearningRate(int id, string description)
		{
			Id = id;
			Description = description;
		}

		public int Id { get; private set; }
		public string Description { get; private set; }

		public static IEnumerable<LearningRate> All()
		{
			return typeof(LearningRate)
					.GetFields(BindingFlags.Public | BindingFlags.Static)
					.Select(f => (LearningRate)f.GetValue(null))
					.ToArray();
		}

		public static LearningRate For(int id)
		{
			if (id < 0 || id > 34)
				return LearningRate.Clear;

			return All().FirstOrDefault(x => x.Id == id);
		}

		public static LearningRate For(string description)
		{
			var result = All().FirstOrDefault(x => x.Description.Equals(description));

			return (result == null || string.IsNullOrWhiteSpace(description))
				? LearningRate.Clear
				: result;
		}
	}
}
