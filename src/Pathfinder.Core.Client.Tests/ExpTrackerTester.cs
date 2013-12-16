using System;
using NUnit.Framework;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class ExpTrackerTester
	{
		private ExpTracker theTracker;

		[SetUp]
		public void SetUp()
		{
			theTracker = new ExpTracker();
		}

		[Test]
		public void calcs_exp_earned()
		{
			var skillExp = new SkillExp
			{
				Name =  "Outdoorsmanship",
				Ranks = "10.00",
				LearningRate = LearningRate.Clear,
				Gained = 0.0
			};

			theTracker.Update(skillExp);

			var cloned = skillExp.Clone();
			cloned.Ranks = "10.05";

			theTracker.Update(cloned);

			Assert.AreEqual("0.05", theTracker.Get("Outdoorsmanship").Gained.ToString("F"));
			Assert.AreEqual("10.00", theTracker.Get("Outdoorsmanship").OriginalRanks);

			cloned.Ranks = "10.10";

			theTracker.Update(cloned);

			Assert.AreEqual("0.10", theTracker.Get("Outdoorsmanship").Gained.ToString("F"));
			Assert.AreEqual("10.10", theTracker.Get("Outdoorsmanship").Ranks);
			Assert.AreEqual("10.00", theTracker.Get("Outdoorsmanship").OriginalRanks);
		}

		[Test]
		public void handles_empty_ranks()
		{
			var skillExp = new SkillExp
			{
				Name =  "Outdoorsmanship",
				Ranks = "10.00",
				LearningRate = LearningRate.Clear,
				Gained = 0.0
			};

			theTracker.Update(skillExp);

			var cloned = skillExp.Clone();
			cloned.Ranks = "10.05";

			theTracker.Update(cloned);

			Assert.AreEqual("0.05", theTracker.Get("Outdoorsmanship").Gained.ToString("F"));

			cloned.Ranks = string.Empty;

			theTracker.Update(cloned);

			Assert.AreEqual("0.05", theTracker.Get("Outdoorsmanship").Gained.ToString("F"));
			Assert.AreEqual("10.05", theTracker.Get("Outdoorsmanship").Ranks);
			Assert.AreEqual("10.00", theTracker.Get("Outdoorsmanship").OriginalRanks);
		}
	}
}
