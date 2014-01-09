using System;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Outlander.Core.Text.Tests
{
	[TestFixture]
	public class ChunkReaderTester
	{
		private ChunkReader<Tag> reader;

		[SetUp]
		public void SetUp()
		{
			reader = new ChunkReader<Tag>("<pushStream", "<popStream", true);
		}

		[Test]
		public void reads_push_stream()
		{
			const string tag = "<pushStream id='inv'/>Your worn items are:\n  a mountain scout's dark ruazin wool robe\n  a diaphanous nightstalker silk uaro's'sugi\n  an archaic warrior's zaulguum-skin backpack woven with Imperial weave straps\n  a well-used first aid kit stamped with herb emblems\n  a broad dragonwood wristcuff\n  a blackened steel parry stick with sturdy steelsilk straps\n  a jadeite gwethdesuan\n  a kyanite gwethdesuan\n  some germish'din hide footwraps\n  some basilisk fang elbow blades\n  some bloodvine thorn knee spikes\n  a wide leather arm cuff branded with the crest of the Ranger guild\n  a dull serpent earcuff\n  a dull serpent earcuff\n  an eagle feather\n  a bloodwood lockpick case inlaid with a charming woodland scene\n  a blackened steel locket\n  a silver key with a spider-web design on the grip\n  a black leather thigh quiver banded with thick dragonwood strips\n  an albredine crystal ring\n  a tan leather hip pouch hand-tooled with a panther stalking a buck\n  a copperwood-hilted wrist knife with a stag carved blade\n  a dark silverweave lootsack\n  some dark leather Elven hunting boots\n  a plain silver torque\n  a xanthic glass prism clutched by a wide Elven silver chain\n  a pair of blackened zills engraved with a flying heron\n  a black leather war belt riveted with blued-steel studs\n  an aged leather outrider's cloak fashioned in an Imperial style\n  a battle-scarred darkened leather baldric tooled with a majestic snow-capped mountain\n  an aged dragonwood scabbard wrapped with Imperial weave\n  a polished leather raekhlo with a damascened haralun sterak axe and a tyrium bastard sword hanging from it\n  a lumpy bundle\n  a scout's small targe\n  some lumium brigandine gloves\n  some shalswar claw handwraps\n  an Imperial battle helm\n  an enameled lamellar shirt adorned with a dragon\n  some lumium plate greaves\n<popStream/>";

			var result = reader.Read(Chunk.For(tag));
			Assert.AreEqual(1, result.Tags.Count());
			Assert.AreEqual(tag, result.Tags.Single().Text);
		}

		[Test]
		public void reads_push_stream_with_surrounding_data()
		{
			const string tag = "<compass><dir value=\"e\"/><dir value=\"nw\"/></compass><pushStream id=\"logons\"/> * Force of Nature Saracus Soulhawk snuck out of the shadow he was hiding in.\n<popStream/><prompt time=\"1385828459\">H&gt;</prompt>";
			const string expected = "<pushStream id=\"logons\"/> * Force of Nature Saracus Soulhawk snuck out of the shadow he was hiding in.\n<popStream/>";

			var tags = reader.Read(Chunk.For(tag));
			Assert.AreEqual(1, tags.Tags.Count());
			Assert.AreEqual(expected, tags.Tags.First().Text);
		}

		[Test]
		public void waits_for_push_stream_between_chunks()
		{
			const string compassTag = "<compass><dir value=\"e\"/><dir value=\"nw\"/></compass>";
			const string tag = "<pushStream id=\"logons\"/> * Force of Nature Saracus Soulhawk snuck out of the shadow he was hiding in.\n";
			const string tag2 = "<popStream/>";
			const string promptTag = "<prompt time=\"1385828459\">H&gt;</prompt>";
			const string expected = "<pushStream id=\"logons\"/> * Force of Nature Saracus Soulhawk snuck out of the shadow he was hiding in.\n<popStream/>";

			var tags = reader.Read(Chunk.For(compassTag + tag));
			Assert.AreEqual(0, tags.Tags.Count());
			Assert.AreEqual(compassTag, tags.Chunk.Text);

			tags = reader.Read(Chunk.For(tag2 + promptTag));
			Assert.AreEqual(1, tags.Tags.Count());
			Assert.AreEqual(expected, tags.Tags.First().Text);
			Assert.AreEqual(promptTag, tags.Chunk.Text);
		}

		[Test]
		public void ignores_push_bold()
		{
			const string tag = "<compass><dir value=\"e\"/><dir value=\"nw\"/></compass><pushStream id=\"logons\"/> * Force of Nature Saracus Soulhawk snuck out of the shadow he was hiding in.\n<popStream/><prompt time=\"1385828459\">H&gt;</prompt>\n<pushBold/>You have outstanding SimuCoin purchases to receive.  Please use SIMUCOIN DELIVER to receive your purchases.\n<popBold/>";
			const string expected = "<pushStream id=\"logons\"/> * Force of Nature Saracus Soulhawk snuck out of the shadow he was hiding in.\n<popStream/>";

			var tags = reader.Read(Chunk.For(tag));
			Assert.AreEqual(1, tags.Tags.Count());
			Assert.AreEqual(expected, tags.Tags.First().Text);
		}

		[Test]
		public void extracts_data_around_push_stream_()
		{
			const string tag = "<compass><dir value=\"e\"/><dir value=\"nw\"/></compass><pushStream id=\"logons\"/> * Force of Nature Saracus Soulhawk snuck out of the shadow he was hiding in.\n<popStream/><prompt time=\"1385828459\">H&gt;</prompt>";
			const string expected = "<compass><dir value=\"e\"/><dir value=\"nw\"/></compass><prompt time=\"1385828459\">H&gt;</prompt>";

			var tags = reader.Read(Chunk.For(tag));
			Assert.AreEqual(1, tags.Tags.Count());
			Assert.AreEqual(expected, tags.Chunk.Text);
		}

		[Test]
		public void extracts_data_between_chunks()
		{
			const string tag = "<compass><dir value=\"e\"/><dir value=\"nw\"/></compass><pushStream id=\"logons\"/> * Force of Nature Saracus Soulhawk snuck out of the shadow he was hiding in.\n";
			const string tag2 = "<popStream/><prompt time=\"1385828459\">H&gt;</prompt>";
			const string expected = "<pushStream id=\"logons\"/> * Force of Nature Saracus Soulhawk snuck out of the shadow he was hiding in.\n<popStream/>";

			var tags = reader.Read(Chunk.For(tag));
			Assert.AreEqual(0, tags.Tags.Count());
			Assert.AreEqual("<compass><dir value=\"e\"/><dir value=\"nw\"/></compass>", tags.Chunk.Text);

			tags = reader.Read(Chunk.For(tag2));
			Assert.AreEqual(1, tags.Tags.Count());
			Assert.AreEqual(expected, tags.Tags.Single().Text);
			Assert.AreEqual("<prompt time=\"1385828459\">H&gt;</prompt>", tags.Chunk.Text);
		}

		[Test]
		public void dailog_data_ignores_other_tags()
		{
			const string compassTag = "<compass><dir value=\"e\"/><dir value=\"nw\"/></compass>";

			var reader = new ChunkReader<Tag>("<dialogData", "</dialogData");

			var result = reader.Read(Chunk.For(compassTag));

			Assert.AreEqual(compassTag, result.Chunk.Text);
		}
	}
}
	