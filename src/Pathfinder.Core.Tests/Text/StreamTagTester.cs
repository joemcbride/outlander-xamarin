using System;
using NUnit.Framework;
using Outlander.Core.Text;

namespace Outlander.Core.Tests
{
	[TestFixture]
	public class StreamTagTester
	{
		[Test]
		public void populates_tag()
		{
			const string data = "<pushStream id=\"assess\"/> You assess your combat situation...\n\n\n<popStream/>";

			var tag = StreamTag.For<StreamTag>(data);

			Assert.AreEqual("assess", tag.Id);
			Assert.AreEqual("You assess your combat situation...\n\n\n", tag.Value);
		}
	}
}
