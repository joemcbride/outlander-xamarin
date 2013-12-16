using System;
using NUnit.Framework;
using Pathfinder.Core.Text;

namespace Pathfinder.Core.Tests
{
	[TestFixture]
	public class AppTagTester
	{
		[Test]
		public void populates_tag()
		{
			const string data = "<app char=\"Tayek\" game=\"DR\" title=\"[DR: Tayek] StormFront\"/>";

			var tag = AppTag.For<AppTag>(data);

			Assert.AreEqual("Tayek", tag.Character);
			Assert.AreEqual("DR", tag.Game);
		}
	}
}
