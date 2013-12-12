using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pathfinder.Core.Text;
using System.Reflection;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Pathfinder.Core.Text.Tests
{	
	[TestFixture]
	public class NewGameParserTester
	{
		private NewGameParser theParser;

		[SetUp]
		public void SetUp()
		{
			theParser = new NewGameParser(new ITagTransformer[]{ new ComponentTagTransformer() });
		}

		[Test]
		public void parses()
		{
			const string compassTag = "<compass><dir value=\"e\"/><dir value=\"nw\"/></compass>";
			const string streamTag = "<pushStream id=\"logons\"/> * Force of Nature Saracus Soulhawk snuck out of the shadow he was hiding in.\n<popStream/>";
			const string promptTag = "<prompt time=\"1385828459\">H&gt;</prompt>";
			const string dialogTag = "<dialogData id='minivitals'><skin id='healthSkin' name='healthBar' controls='health' left='0%' top='0%' width='20%' height='100%'/><progressBar id='health' value='100' text='health 100%' left='0%' customText='t' top='0%' width='20%' height='100%'/></dialogData>";
			const string roundtimeTag = "<roundTime value='1366812703'/>";
			const string leftTag = "<left>Empty</left>";
			const string rightTag = "<right>Empty</right>";

			var chunk = Chunk.For(compassTag + streamTag + roundtimeTag + promptTag + leftTag + rightTag + dialogTag);

			var result = theParser.Parse(chunk);

			var results = result.Tags.ToList();

			Assert.AreEqual(7, results.Count);
			Assert.AreEqual(streamTag, results[0].Text);
			Assert.AreEqual(promptTag, results[1].Text);
			Assert.AreEqual(dialogTag, results[2].Text);
			Assert.AreEqual(compassTag, results[3].Text);
			Assert.AreEqual(roundtimeTag, results[4].Text);
			Assert.AreEqual(leftTag, results[5].Text);
			Assert.AreEqual(rightTag, results[6].Text);
		}

		[Test]
		public void handles_login()
		{
			const string expected = "Please wait for connection to game server.\nGSw000100000553733\nWelcome to DragonRealms (R) v2.00\nCopyright 2013 Simutronics Corp.\nAll Rights Reserved\n\n\n\n\n<output class=\"mono\"/>\n\n----------------------------------------------------------------------------\n   Last login :  Saturday, December 7, 2013 at 16:54:49\n       Logoff :  Saturday, December 7, 2013 at 16:55:05\n----------------------------------------------------------------------------\n\n<output class=\"\"/>\n[Woodland Brook]\nWater ripples rapidly around a rough-bark log of an old willow tree protruding out of the brook at an angle.  Periwinkle creepers twist up the trunk, decorating it with deep purple flowers.  High up on the log a kingfisher has made a nest, and occasionally the tiny blue and orange bird pokes an inquisitive head out and eyes the brook for prey.\nObvious paths: <d>northeast</d>, <d>south</d>, <d>northwest</d>.\n";

			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Pathfinder.Core.Tests.Data.login-data1.txt"))
			using(var reader = new StreamReader(stream))
			{
				var data = reader.ReadToEnd();
				string[] stringChunks = data.Split(new string[]{ "^---------------------^" }, StringSplitOptions.None);
				var chunks = stringChunks.Select(c => Chunk.For(c)).ToList();

				var builder = new StringBuilder();
				var tags = new List<Tag>();

				for (int i = 0; i < chunks.Count; i++) {
					var chunk = chunks[i];
					var result = theParser.Parse(chunk);
					if(result.Chunk != null)
						builder.Append(result.Chunk.Text.TrimStart());

					tags.AddRange(result.Tags);
				}

				Assert.AreEqual(145, tags.Count);
				Assert.AreEqual(expected, builder.ToString());
			}
		}

		[Test]
		public void transforms_tags()
		{
			const string data = "<component id='room exits'>Obvious paths: <d>northeast</d>, <d>south</d>, <d>northwest</d>.<compass></compass></component>";
			const string expected = "Obvious paths: northeast, south, northwest.";

			theParser = new NewGameParser(new ITagTransformer[]{ new ComponentTagTransformer() });

			var result = theParser.Parse(Chunk.For(data));

			Assert.AreEqual(expected, result.Tags.OfType<ComponentTag>().Single().Value);
		}

		[Test]
		public void transforms_tags_2()
		{
			const string data = "<streamWindow id='room' title='Room' subtitle=\" - [Grassland Road, Farmlands]\" location='center' target='drop' ifClosed='' resident='true'/>";
			const string expected = "[Grassland Road, Farmlands]";

			theParser = new NewGameParser(new ITagTransformer[]{ new StreamWindowTagTransformer() });

			var result = theParser.Parse(Chunk.For(data));

			Assert.AreEqual(expected, result.Tags.OfType<ComponentTag>().Single().Value);
		}

		[Test]
		public void transforms_tags_3()
		{
			const string data = "<component id='room objs'>You also see <pushBold/>a musk hog<popBold/>.</component>";
			const string expected = "You also see <pushBold/>a musk hog<popBold/>.";

			theParser = new NewGameParser(Enumerable.Empty<ITagTransformer>());

			var result = theParser.Parse(Chunk.For(data));

			Assert.AreEqual(expected, result.Tags.OfType<ComponentTag>().Single().Value);
		}

		[Test]
		public void shows_full_room()
		{
			const string data = "<resource picture=\"0\"/><style id=\"roomName\" />[Grassland Road, Meadow]\n<style id=\"\"/><preset id='roomDesc'>The vines of dewberry and blooming trumpet creeper twist and intertwine along the edge of the road, separating it from the vast stretch of grassland beyond.  Far off to the east you can see the building tops of a large town.</preset>  You also see <pushBold/>a musk hog<popBold/>, <pushBold/>a musk hog<popBold/>, <pushBold/>a musk hog<popBold/> and <pushBold/>a musk hog<popBold/>.\nAlso here: Zapthozorr.\nObvious paths: <d>southeast</d>, <d>west</d>.";
			const string expected = "[Grassland Road, Meadow]\nThe vines of dewberry and blooming trumpet creeper twist and intertwine along the edge of the road, separating it from the vast stretch of grassland beyond.  Far off to the east you can see the building tops of a large town.\nYou also see <pushBold/>a musk hog<popBold/>, <pushBold/>a musk hog<popBold/>, <pushBold/>a musk hog<popBold/> and <pushBold/>a musk hog<popBold/>.\nAlso here: Zapthozorr.\nObvious paths: <d>southeast</d>, <d>west</d>.";

			var result = theParser.Parse(Chunk.For(data));

			Assert.AreEqual(expected, result.Chunk.Text);
		}

		[Test]
		public void obvious_paths_reader()
		{
			const string data = "\n\nObvious paths: <d>northeast</d>, <d>south</d>, <d>northwest</d>.";
			const string expected = "\nObvious paths: <d>northeast</d>, <d>south</d>, <d>northwest</d>.";

			var result = theParser.Parse(Chunk.For(data));

			Assert.AreEqual(expected, result.Chunk.Text);
		}

		[Test]
		public void ensure_components_parsed_before_preset()
		{
			const string start = "<component id='exp Athletics'><preset id='whisper'>       Athletics:   21 30% analyzing    </preset></component>\n";
			const string data = "Roundtime: 5 sec.\nYou swim south, moving with the light current.\n<nav/>\n<streamWindow id='main' title='Story' subtitle=\" - [Woodland Path, Brook]\" location='center' target='drop'/>\n<streamWindow id='room' title='Room' subtitle=\" - [Woodland Path, Brook]\" location='center' target='drop' ifClosed='' resident='true'/>\n<component id='room desc'>This shallow stream would probably only come chest-high on a short Halfling.  The water moves lazily southward, but the shifting, sharp rocky floor makes crossing uncomfortable.</component>\n<component id='room objs'></component>\n<component id='room players'></component>\n<component id='room exits'>Obvious paths: <d>north</d>, <d>east</d>, <d>west</d>.<compass></compass></component>\n<component id='room extra'></component>\n<resource picture=\"0\"/><style id=\"roomName\" />[Woodland Path, Brook]\n";
			const string expected = "Roundtime: 5 sec.\nYou swim south, moving with the light current.\n\n\n\n\n\n\n\n\n";

			const string next = "<style id=\"\"/><preset id='roomDesc'>This shallow stream would probably only come chest-high on a short Halfling.  The water moves lazily southward, but the shifting, sharp rocky floor makes crossing uncomfortable.</preset>  \nObvious paths: <d>north</d>, <d>east</d>, <d>west</d>.\n";
			const string expected2 = "[Woodland Path, Brook]\nThis shallow stream would probably only come chest-high on a short Halfling.  The water moves lazily southward, but the shifting, sharp rocky floor makes crossing uncomfortable.\nObvious paths: <d>north</d>, <d>east</d>, <d>west</d>.\n";

			var result = theParser.Parse(Chunk.For(start));

			Assert.AreEqual(1, result.Tags.Count());

			result = theParser.Parse(Chunk.For(data));

			Assert.AreEqual(expected, result.Chunk.Text);
			Assert.AreEqual(9, result.Tags.Count());

			result = theParser.Parse(Chunk.For(next));
			Assert.AreEqual(2, result.Tags.Count());
			Assert.AreEqual(expected2, result.Chunk.Text);
		}

		[Test]
		public void handles_roomname_and_paths()
		{
			const string data = "<resource picture=\"0\"/><style id=\"roomName\" />[Woodland Brook]\n<style id=\"\"/><preset id='roomDesc'>Water ripples rapidly around a rough-bark log of an old willow tree protruding out of the brook at an angle.  Periwinkle creepers twist up the trunk, decorating it with deep purple flowers.  High up on the log a kingfisher has made a nest, and occasionally the tiny blue and orange bird pokes an inquisitive head out and eyes the brook for prey.</preset>  \nObvious paths: <d>northeast</d>, <d>south</d>, <d>northwest</d>.\n<compass><dir value=\"ne\"/><dir value=\"s\"/><dir value=\"nw\"/></compass><component id='room players'></component>\n<prompt time=\"1386489933\">R&gt;</prompt>\n";
			const string expected = "[Woodland Brook]\nWater ripples rapidly around a rough-bark log of an old willow tree protruding out of the brook at an angle.  Periwinkle creepers twist up the trunk, decorating it with deep purple flowers.  High up on the log a kingfisher has made a nest, and occasionally the tiny blue and orange bird pokes an inquisitive head out and eyes the brook for prey.\nObvious paths: <d>northeast</d>, <d>south</d>, <d>northwest</d>.\n\n\n";

			var result = theParser.Parse(Chunk.For(data));

			Assert.AreEqual(expected, result.Chunk.Text);
		}
	}
}
