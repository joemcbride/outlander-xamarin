using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;

namespace Pathfinder.Core
{
	public class SkillExp
	{
		private const string Rank_Regex = "(\\d+)\\s(\\d+)%\\s(\\w.*)";

		public string Name { get; set; }
		public string Ranks { get; set; }
		public string OriginalRanks { get; set; }
		public LearningRate LearningRate { get; set; }
		public double Gained { get; set; }

		public bool IsNew { get; set; }

		public SkillExp Clone()
		{
			return new SkillExp
			{
				Name = Name,
				Ranks = Ranks,
				OriginalRanks = OriginalRanks,
				LearningRate = LearningRate,
				Gained = Gained
			};
		}

		public static SkillExp For(ComponentTag tag)
		{
			var exp = new SkillExp();
			exp.Name = tag.Id;
			exp.Ranks = Regex.Replace(tag.Value, Rank_Regex, "$1.$2");

			var mindState = Regex.Replace(tag.Value, Rank_Regex, "$3");
			exp.LearningRate = LearningRate.For(mindState);

			exp.IsNew = tag.IsNewExp;

			return exp;
		}
	}
}
