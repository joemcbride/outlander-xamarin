using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outlander.Core;

namespace Outlander.Core.Client
{
	public class ExpTracker
	{
		private IDictionary<string, SkillExp> _skills = new Dictionary<string, SkillExp>();

		public DateTime? StartedTracking { get; private set; }

		public void Update(SkillExp skill)
		{
			if(!StartedTracking.HasValue)
				StartedTracking = DateTime.Now;

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
				target.OriginalRanks = skill.Ranks;
				_skills[skill.Name] = target;
			}

			if(string.IsNullOrWhiteSpace(target.OriginalRanks))
			{
				target.OriginalRanks = skill.Ranks;
			}

			if(calcDiff && !string.IsNullOrWhiteSpace(target.Ranks) && !string.IsNullOrWhiteSpace(skill.Ranks))
			{
				target.Gained = Difference(target.OriginalRanks, skill.Ranks);
			}

			if(!string.IsNullOrWhiteSpace(skill.Ranks)) {
				target.Ranks = skill.Ranks;
			}
			target.LearningRate = skill.LearningRate;
			target.IsNew = skill.IsNew;
		}

		public SkillExp Get(string skillName)
		{
			return _skills[skillName];
		}

		public IEnumerable<SkillExp> All()
		{
			return _skills.Values;
		}

		public IEnumerable<SkillExp> SkillsWithExp()
		{
			return _skills.Values.Where(x => x.LearningRate.Id > 0);
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
