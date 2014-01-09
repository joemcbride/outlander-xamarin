using System;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Outlander.Core.Text.Tests
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

		[Test]
		public void reads_multiple_tags_in_same_chunk()
		{
			const string testData = "<streamWindow id=\"death\" title=\"Deaths\" location=\"left\" resident=\"true\" nameFilterOption=\"true\" timestamp=\"on\"/>\n<streamWindow id=\"assess\" title=\"Assess\" location=\"right\" resident=\"true\"/>\n<streamWindow id='conversation' title='Conversation' location='center' resident='true' ifClosed='' timestamp='on'/>\n<streamWindow id='whispers' title='Whispers' location='center' resident='true' timestamp='on' ifClosed='conversation'/>\n<streamWindow id='talk' title='Talk' location='center' resident='true' timestamp='on' ifClosed='conversation'/>\n<streamWindow id='experience' title='Field Experience' location='center' target='drop' ifClosed='' resident='true'/><clearStream id='experience'/><pushStream id='experience'/><output class='mono'/>\n<compDef id='exp Shield Usage'></compDef>\n<compDef id='exp Light Armor'></compDef>\n<compDef id='exp Chain Armor'></compDef>\n<compDef id='exp Brigandine'></compDef>\n<compDef id='exp Plate Armor'></compDef>\n<compDef id='exp Defending'></compDef>\n<compDef id='exp Parry Ability'></compDef>\n";

			theReader = new SelfClosingChunkReader<Tag>("<streamWindow");

			var result = theReader.Read(Chunk.For(testData));

			Assert.AreEqual(6, result.Tags.Count());
		}
	}
}
	