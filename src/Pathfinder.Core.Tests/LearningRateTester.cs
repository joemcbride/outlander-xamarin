using System;
using System.Linq;
using NUnit.Framework;

namespace Pathfinder.Core.Tests
{
	[TestFixture]
	public class LearningRateTester
	{
		[Test]
		public void retrieves_all()
		{
			Assert.AreEqual(35, LearningRate.All().Count());
		}

		[Test]
		public void retrieves_clear()
		{
			Assert.AreEqual(LearningRate.For(0), LearningRate.Clear);
		}

		[Test]
		public void defaults_to_clear_for_unkown()
		{
			Assert.AreEqual(LearningRate.For(57), LearningRate.Clear);
			Assert.AreEqual(LearningRate.For(-1), LearningRate.Clear);
			Assert.AreEqual(LearningRate.For(string.Empty), LearningRate.Clear);
			Assert.AreEqual(LearningRate.For("abcd"), LearningRate.Clear);
		}
	}
}
