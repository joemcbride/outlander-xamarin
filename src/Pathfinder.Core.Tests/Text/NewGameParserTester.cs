using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Pathfinder.Core.Text;

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
			using (var chunkStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Pathfinder.Core.Tests.Data.login-data1.txt"))
			using (var expectedStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Pathfinder.Core.Tests.Data.login-data1-expected.txt"))
			using (var chunkReader = new StreamReader(chunkStream))
			using (var expectedReader = new StreamReader(expectedStream))
			{
				var data = chunkReader.ReadToEnd();
				var expected = expectedReader.ReadToEnd();

				var result = BuildResult(data);

				Assert.AreEqual(145, result.Tags.Count);
				Assert.AreEqual(expected, result.Builder.ToString());
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
			const string expected = "Roundtime: 5 sec.\nYou swim south, moving with the light current.\n\n";

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
			const string expected = "[Woodland Brook]\nWater ripples rapidly around a rough-bark log of an old willow tree protruding out of the brook at an angle.  Periwinkle creepers twist up the trunk, decorating it with deep purple flowers.  High up on the log a kingfisher has made a nest, and occasionally the tiny blue and orange bird pokes an inquisitive head out and eyes the brook for prey.\nObvious paths: <d>northeast</d>, <d>south</d>, <d>northwest</d>.\nR>\n";

			var result = theParser.Parse(Chunk.For(data));

			Assert.AreEqual(expected, result.Chunk.Text);
		}

		[Test]
		public void handles_boarding_ferry()
		{
			const string data = "[Assuming you mean the ferry Hodierna's Grace.]\nThe Captain stops you and requests a transportation fee of 35 kronars as you board the craft.\nYou hand him your kronars and climb aboard.\n\n<nav/>\n<streamWindow id='main' title='Story' subtitle=\" - [\"Hodierna's Grace\"]\" location='center' target='drop'/>\n<streamWindow id='room' title='Room' subtitle=\" - [\"Hodierna's Grace\"]\" location='center' target='drop' ifClosed='' resident='true'/>\n<component id='room desc'>A few weary travelers lean against a railing at the bow of this ferry, anxiously waiting to reach the opposite bank.  An elderly S'Kra Mur stands alone at the stern, thoughtfully watching the shallow wake of the ferry shiver and become still.</component>\n<component id='room objs'>You also see the north bank docks.</component>\n<component id='room players'></component>\n<component id='room exits'>Obvious paths: none.<compass></compass></component>\n<component id='room extra'></component>";
			const string expected = "[Assuming you mean the ferry Hodierna's Grace.]\nThe Captain stops you and requests a transportation fee of 35 kronars as you board the craft.\nYou hand him your kronars and climb aboard.";

			var result = theParser.Parse(Chunk.For(data));

			Assert.AreEqual(expected, result.Chunk.Text.TrimEnd());
		}

		[Test]
		public void displays_prompts_correctly_with_look_at_character()
		{
			const string data = "<prompt time=\"1388365633\">&gt;</prompt>\n^---------------------^You see Someone, an Elothean.\nSomeone has an oval face, almond-shaped gold eyes and a straight nose.  He is bald, with tanned skin and a stout build.\nHe is short for an Elothean.\nHe appears to be young.\nHe has a long narrow mustache that droops to either side of the mouth on his upper lip and a long thin beard.\nHe is in good shape.\n\n^---------------------^He is wearing some coarse shalswar-hide leathers, a coarse shalswar-hide cowl, a gargoyle-hide targe, some lumium ring gloves, a green gem pouch, a pilgrim's badge, a thin oblong polished silver flask, a kertig prayer bead chain interspersed with faceted black diamond beads, a sandstone lockpick ring, some fitted silver trousers, a flowing silk cloak bejeweled with the crest of the Clerics' guild, a silver kyanite gwethdesuan, a silver jadeite gwethdesuan, a rusted iron skull suspended from a battered leather cord, a dark red spidersilk haversack clasped with a platinum clenched fist, an ornate platinum brooch set with an orichalcum icosahedron, a dark cambrinth armband inset with a multitude of tiny gemstone chips, an anloral wolf pin, a heavy leather pouch bearing the crest of the Clerics' Guild, a worn grey hitman's backpack with fraying straps and a slender alabaster staff with a silver cobra coiled along it.\n<prompt time=\"1388365636\">&gt;</prompt>";
			const string expected = ">\nYou see Someone, an Elothean.\nSomeone has an oval face, almond-shaped gold eyes and a straight nose.  He is bald, with tanned skin and a stout build.\nHe is short for an Elothean.\nHe appears to be young.\nHe has a long narrow mustache that droops to either side of the mouth on his upper lip and a long thin beard.\nHe is in good shape.\n\nHe is wearing some coarse shalswar-hide leathers, a coarse shalswar-hide cowl, a gargoyle-hide targe, some lumium ring gloves, a green gem pouch, a pilgrim's badge, a thin oblong polished silver flask, a kertig prayer bead chain interspersed with faceted black diamond beads, a sandstone lockpick ring, some fitted silver trousers, a flowing silk cloak bejeweled with the crest of the Clerics' guild, a silver kyanite gwethdesuan, a silver jadeite gwethdesuan, a rusted iron skull suspended from a battered leather cord, a dark red spidersilk haversack clasped with a platinum clenched fist, an ornate platinum brooch set with an orichalcum icosahedron, a dark cambrinth armband inset with a multitude of tiny gemstone chips, an anloral wolf pin, a heavy leather pouch bearing the crest of the Clerics' Guild, a worn grey hitman's backpack with fraying straps and a slender alabaster staff with a silver cobra coiled along it.\n>";

			var result = BuildResult(data);

			Assert.AreEqual(expected, result.Builder.ToString());
		}

		[Test]
		public void displays_room_without_extra_linebreaks()
		{
			const string data = "You go southeast.\n<nav/>\n<streamWindow id='main' title='Story' subtitle=\" - [Mycthengelde, Flatlands]\" location='center' target='drop'/>\n<streamWindow id='room' title='Room' subtitle=\" - [Mycthengelde, Flatlands]\" location='center' target='drop' ifClosed='' resident='true'/>\n<component id='room desc'>Tree boughs swaying overhead appear to sweep the stars from the sky from time to time.  Dried leaves on the trail crunch beneath your feet in the darkness.</component>\n<component id='room objs'></component>\n<component id='room players'></component>\n<component id='room exits'>Obvious paths: <d>southeast</d>, <d>northwest</d>.<compass></compass></component>\n<component id='room extra'></component>\n<resource picture=\"0\"/><style id=\"roomName\" />[Mycthengelde, Flatlands]\n<style id=\"\"/><preset id='roomDesc'>Tree boughs swaying overhead appear to sweep the stars from the sky from time to time.  Dried leaves on the trail crunch beneath your feet in the darkness.</preset>  \n^---------------------^Obvious paths: <d>southeast</d>, <d>northwest</d>.\n<compass><dir value=\"se\"/><dir value=\"nw\"/></compass><component id='room players'></component>\n<prompt time=\"1388453456\">&gt;</prompt>\n";
			const string expected = "You go southeast.\n\n[Mycthengelde, Flatlands]\nTree boughs swaying overhead appear to sweep the stars from the sky from time to time.  Dried leaves on the trail crunch beneath your feet in the darkness.\nObvious paths: <d>southeast</d>, <d>northwest</d>.\n>\n";

			var result = BuildResult(data);

			Assert.AreEqual(expected, result.Builder.ToString());
		}

		[Test]
		public void displays_who_full_correctly()
		{
			using (var chunkStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Pathfinder.Core.Tests.Data.who-full-chunks.txt"))
			using (var expectedStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Pathfinder.Core.Tests.Data.who-full-expected.txt"))
			using (var chunkReader = new StreamReader(chunkStream))
			using (var expectedReader = new StreamReader(expectedStream))
			{
				var data = chunkReader.ReadToEnd();
				var expected = expectedReader.ReadToEnd();

				var result = BuildResult(data);

				Assert.AreEqual(expected, result.Builder.ToString());
			}
		}

		[Test]
		public void removes_some_prompts()
		{
			const string data = "<resource picture=\"0\"/><style id=\"roomName\" />[Grassland Road, Meadow]\n<style id=\"\"/><preset id='roomDesc'>Tall sharp blades of switch grass rustle and whisper in the breeze.  Here and there small clumps of wild sweet cicely grow in the meadow, filling the air with a spicy scent.</preset>  You also see <pushBold/>a musk hog<popBold/>.\nObvious paths: <d>east</d>, <d>west</d>.\n<compass><dir value=\"e\"/><dir value=\"w\"/></compass><prompt time=\"1388478977\">&gt;</prompt>\n^---------------------^The musk hog begins to advance on you!\nThe musk hog is still a distance away from you and is closing steadily.\n<prompt time=\"1388478979\">&gt;</prompt>\n^---------------------^The musk hog closes to pole weapon range on you!\n<prompt time=\"1388478987\">&gt;</prompt>\n^---------------------^The musk hog closes to melee range on you!\n<prompt time=\"1388478991\">&gt;</prompt>\n^---------------------^<component id='exp Evasion'><preset id='whisper'>         Evasion:   26 65% thoughtful   </preset></component>\n<component id='exp Parry Ability'><preset id='whisper'>   Parry Ability:   21 33% learning     </preset></component>\n<component id='exp Large Edged'><preset id='whisper'>     Large Edged:   21 19% perusing     </preset></component>\n<component id='exp Defending'><preset id='whisper'>       Defending:   23 12% perusing     </preset></component>\n<component id='exp Light Armor'><preset id='whisper'>     Light Armor:   27 07% thoughtful   </preset></component>\nA musk hog hisses viciously and lunges at you!\n* Ineptly, a musk hog claws, swiping with its hooves at you.  You evade, ducking in the nick of time.  \n[You're solidly balanced and have slight advantage.]\n<prompt time=\"1388478991\">&gt;</prompt>\n^---------------------^<dialogData id='minivitals'><skin id='manaSkin' name='manaBar' controls='mana' left='20%' top='0%' width='20%' height='100%'/><progressBar id='mana' value='100' text='mana 100%' left='20%' customText='t' top='0%' width='20%' height='100%'/></dialogData>\n^---------------------^<prompt time=\"1388478997\">&gt;</prompt>\n<component id='room objs'>You also see <pushBold/>a musk hog<popBold/>.</component>\n<prompt time=\"1388478997\">&gt;</prompt>\n<component id='exp Evasion'><preset id='whisper'>         Evasion:   26 65% thinking     </preset></component>\n<component id='exp Parry Ability'><preset id='whisper'>   Parry Ability:   21 33% learning     </preset></component>\n<component id='exp Large Edged'><preset id='whisper'>     Large Edged:   21 19% perusing     </preset></component>\n<component id='exp Defending'><preset id='whisper'>       Defending:   23 12% perusing     </preset></component>\n<component id='exp Light Armor'><preset id='whisper'>     Light Armor:   27 07% thoughtful   </preset></component>\n<pushBold/><popBold/>A musk hog hisses viciously and lunges at you!\n* Apparently without direction or thought, a musk hog claws, swiping with its hooves at you.  You knock aside some of the claw with a longsword.  \n[You're solidly balanced and in good position.]\n^---------------------^<prompt time=\"1388478997\">&gt;</prompt>";
			const string expected = "[Grassland Road, Meadow]\nTall sharp blades of switch grass rustle and whisper in the breeze.  Here and there small clumps of wild sweet cicely grow in the meadow, filling the air with a spicy scent.\nYou also see <pushBold/>a musk hog<popBold/>.\nObvious paths: <d>east</d>, <d>west</d>.\n>\nThe musk hog begins to advance on you!\nThe musk hog is still a distance away from you and is closing steadily.\n>\nThe musk hog closes to pole weapon range on you!\n>\nThe musk hog closes to melee range on you!\n>\nA musk hog hisses viciously and lunges at you!\n* Ineptly, a musk hog claws, swiping with its hooves at you.  You evade, ducking in the nick of time.  \n[You're solidly balanced and have slight advantage.]\n>\n<pushBold/><popBold/>A musk hog hisses viciously and lunges at you!\n* Apparently without direction or thought, a musk hog claws, swiping with its hooves at you.  You knock aside some of the claw with a longsword.  \n[You're solidly balanced and in good position.]\n";

			var result = BuildResult(data);

			Assert.AreEqual(expected, result.Builder.ToString());
		}

		[Test]
		public void removes_some_prompts_2()
		{
			const string data = "<prompt time=\"1388478991\">&gt;</prompt>\n^---------------------^<dialogData id='minivitals'><skin id='manaSkin' name='manaBar' controls='mana' left='20%' top='0%' width='20%' height='100%'/><progressBar id='mana' value='100' text='mana 100%' left='20%' customText='t' top='0%' width='20%' height='100%'/></dialogData>\n^---------------------^<prompt time=\"1388478997\">&gt;</prompt>\n<component id='room objs'>You also see <pushBold/>a musk hog<popBold/>.</component>\n<prompt time=\"1388478997\">&gt;</prompt>\n<component id='exp Evasion'><preset id='whisper'>         Evasion:   26 65% thinking     </preset></component>\n<component id='exp Parry Ability'><preset id='whisper'>   Parry Ability:   21 33% learning     </preset></component>\n<component id='exp Large Edged'><preset id='whisper'>     Large Edged:   21 19% perusing     </preset></component>\n<component id='exp Defending'><preset id='whisper'>       Defending:   23 12% perusing     </preset></component>\n<component id='exp Light Armor'><preset id='whisper'>     Light Armor:   27 07% thoughtful   </preset></component>\n<pushBold/><popBold/>A musk hog hisses viciously and lunges at you!\n* Apparently without direction or thought, a musk hog claws, swiping with its hooves at you.  You knock aside some of the claw with a longsword.  \n[You're solidly balanced and in good position.]\n";
			const string expected = ">\n>\n<pushBold/><popBold/>A musk hog hisses viciously and lunges at you!\n* Apparently without direction or thought, a musk hog claws, swiping with its hooves at you.  You knock aside some of the claw with a longsword.  \n[You're solidly balanced and in good position.]\n";

			var result = BuildResult(data);

			Assert.AreEqual(expected, result.Builder.ToString());
		}

		[Test]
		public void parses_thoughts()
		{
			const string data = "<dialogData id='minivitals'><progressBar id='concentration' value='98' text='concentration 98%' left='80%' customText='t' top='0%' width='20%' height='100%'/></dialogData>\n^---------------------^<pushStream id=\"thoughts\"/><preset id='thought'>You hear your mental voice echo, </preset>\"Testing, one, two.\"\n<popStream/>You concentrate on projecting your thoughts.\n<prompt time=\"1388893365\">&gt;</prompt>\n";

			var result = BuildResult(data);

			Assert.AreEqual(3, result.Tags.Count);
			Assert.AreEqual("thoughts", result.Tags[1].As<StreamTag>().Id);
		}

		[Test]
		public void parses_exp_tdp()
		{
			const string data = "<component id='exp tdp'>            TDPs:  197</component>";
			const string expected = "";

			var result = BuildResult(data);

			Assert.AreEqual(1, result.Tags.Count);
			var tag = result.Tags[0].As<ComponentTag>();
			Assert.AreEqual("tdp", tag.Id);
			Assert.AreEqual("197", tag.Value);
		}

		private TestParseResult BuildResult(string data)
		{
			string[] stringChunks = data.Split(new string[]{ "^---------------------^" }, StringSplitOptions.None);
			var chunks = stringChunks.Select(c => Chunk.For(c)).ToList();

			var builder = new StringBuilder();
			var tags = new List<Tag>();

			for (int i = 0; i < chunks.Count; i++) {
				var chunk = chunks[i];
				var result = theParser.Parse(chunk);
				if(result.Chunk != null)
					builder.Append(result.Chunk.Text);

				tags.AddRange(result.Tags);
			}

			return new TestParseResult { Builder = builder, Tags = tags };
		}
	}

	public class TestParseResult
	{
		public StringBuilder Builder { get; set; }
		public List<Tag> Tags { get; set; }
	}
}
