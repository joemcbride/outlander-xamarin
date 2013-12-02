using System;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Pathfinder.Core.Text.Tests
{


	[TestFixture]
	public class SelfClosingChunkReaderTester
	{
		private IChunkReader theReader;

		[SetUp]
		public void SetUp()
		{
			theReader = new SelfClosingChunkReader<Tag>("<roundTime");
		}

		[Test]
		public void reads_roundtime()
		{
			const string tag = "<roundTime value='1366812703'/>";
			var result = theReader.Read(Chunk.For(tag));

			Assert.AreEqual(tag, result.Tags.Single().Text);
		}

		[Test]
		public void reads_tag_admist_other_full_tags()
		{
			const string compassTag = "<compass><dir value=\"e\"/><dir value=\"nw\"/></compass>";
			const string promptTag = "<prompt time=\"1385828459\">H&gt;</prompt>";
			const string roundtimeTag = "<roundTime value='1366812703'/>";

			var result = theReader.Read(Chunk.For(compassTag + roundtimeTag + promptTag));

			Assert.AreEqual(roundtimeTag, result.Tags.Single().Text);
		}
	}
}
	