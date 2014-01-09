using System;
using NUnit.Framework;
using Outlander.Core.Text;

namespace Outlander.Core.Tests
{
	[TestFixture]
	public class SkillExpTester
	{
		[Test]
		public void parses_skill_info()
		{
			const string tag = "<component id='exp Outdoorsmanship'><preset id='whisper'>        Outdoorsmanship:   23 27% mind lock    </preset></component>";

			var skill = SkillExp.For(ComponentTag.For<ComponentTag>(tag));
			Assert.AreEqual("Outdoorsmanship", skill.Name);
			Assert.AreEqual("23.27", skill.Ranks);
			Assert.True(skill.IsNew);
			Assert.AreEqual(LearningRate.MindLock, skill.LearningRate);
		}
	}
}
