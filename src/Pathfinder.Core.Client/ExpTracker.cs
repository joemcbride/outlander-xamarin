using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder.Core.Client
{
	public class ExpTracker
	{
		private IDictionary<string, SkillExp> _skills = new Dictionary<string, SkillExp>();

		public void Update(SkillExp skill)
		{
			SkillExp target = null;
			bool calcDiff = false;

			if(_skills.ContainsKey(skill.Name))
			{
				target = _skills[skill.Name];
				calcDiff = true;
			}
			else
			{
				target = skill;
				_skills[skill.Name] = target;
			}

			if(calcDiff && !string.IsNullOrWhiteSpace(target.Ranks) && !string.IsNullOrWhiteSpace(skill.Ranks))
			{
				target.Gained = Difference(target.Ranks, skill.Ranks);
			}

			if(!string.IsNullOrWhiteSpace(skill.Ranks)) {
				target.Ranks = skill.Ranks;
			}
			target.LearningRate = skill.LearningRate;
		}

		public IEnumerable<SkillExp> All()
		{
			return _skills.Values;
		}

		public IEnumerable<SkillExp> SkillsWithExp()
		{
			return _skills.Values.Where(x => x.LearningRate.Id > 0);
		}

		public string StringDisplay()
		{
			var builder = new StringBuilder();

			SkillsWithExp()
				.OrderBy(x=>x.Name)
				.Apply(exp =>
					builder.AppendLine(
						string.Format("{0,-15} {1,7} {2,3}/34 {3}{4:F}",
							exp.Name.Replace("_", " "),
							exp.Ranks,
							exp.LearningRate.Id,
							PosNeg(exp.Gained),
							exp.Gained)));

			return builder.ToString();
		}

		private string PosNeg(double gained)
		{
			if(gained == 0)
				return " ";

			return gained > 0 ? "+" : "-";
		}

		private double ParseRanks(string ranks)
		{
			return string.IsNullOrWhiteSpace(ranks) ? 0.0 : double.Parse(ranks);
		}

		private double Difference(string ranks1, string ranks2)
		{
			var dRanks1 = ParseRanks(ranks1);
			var dRanks2 = ParseRanks(ranks2);

			return dRanks2 - dRanks1;
		}
	}
}
