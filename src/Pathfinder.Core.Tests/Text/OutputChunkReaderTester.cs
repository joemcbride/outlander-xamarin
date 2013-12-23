using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Pathfinder.Core.Text;

namespace Pathfinder.Core.Tests
{
	[TestFixture]
	public class OutputChunkReaderTester
	{
		private OutputChunkReader theReader;

		[SetUp]
		public void SetUp()
		{
			theReader = new OutputChunkReader();
		}

		[Test]
		public void spits_out_chunks()
		{
			const string expected1 = "<output class=\"mono\"/>\n\nCircle: 3\nShowing all skills regardless of rank.\n\n          SKILL: Rank/Percent towards next rank/Amount learning/Mindstate Fraction\n    Shield Usage:      9 56% clear          (0/34)     Light Armor:     10 19% clear          (0/34)\n     Chain Armor:     10 17% clear          (0/34)      Brigandine:      7 88% clear          (0/34)\n     Plate Armor:      2 00% clear          (0/34)       Defending:      9 70% clear          (0/34)\n   Parry Ability:      8 99% clear          (0/34)     Small Edged:     11 16% clear          (0/34)\n     Large Edged:     10 21% clear          (0/34) Twohanded Edged:      3 00% clear          (0/34)\n     Small Blunt:      7 05% clear          (0/34)     Large Blunt:      3 00% clear          (0/34)\n Twohanded Blunt:      3 00% clear          (0/34)          Slings:      3 00% clear          (0/34)\n             Bow:      3 00% clear          (0/34)        Crossbow:      3 00% clear          (0/34)\n<output class=\"\"/>";
			const string expected2 = "<output class=\"mono\"/>          Staves:      3 00% clear          (0/34)        Polearms:      3 00% clear          (0/34)\n    Light Thrown:      4 14% clear          (0/34)    Heavy Thrown:      3 00% clear          (0/34)\n        Brawling:      5 93% clear          (0/34)  Offhand Weapon:      1 50% clear          (0/34)\n   Melee Mastery:     10 49% clear          (0/34) Missile Mastery:      1 38% clear          (0/34)\n Elemental Magic:     12 28% clear          (0/34)      Attunement:     25 27% clear          (0/34)\n          Arcana:     17 28% clear          (0/34)  Targeted Magic:     12 29% clear          (0/34)\n    Augmentation:     13 42% clear          (0/34)    Debilitation:      4 00% clear          (0/34)\n         Utility:      4 00% clear          (0/34)         Warding:      4 00% clear          (0/34)\n         Sorcery:     14 71% clear          (0/34)       Summoning:     18 16% clear          (0/34)\n         Evasion:      9 91% clear          (0/34)       Athletics:     27 97% dabbling       (1/34)\n<output class=\"\"/>";
			const string expected3 = "<output class=\"mono\"/>      Perception:     13 56% clear          (0/34)         Stealth:      8 14% clear          (0/34)\n    Locksmithing:      4 06% clear          (0/34)        Thievery:      0 00% clear          (0/34)\n       First Aid:      1 00% clear          (0/34) Outdoorsmanship:     17 62% clear          (0/34)\n        Skinning:      9 99% clear          (0/34)         Forging:      0 00% clear          (0/34)\n     Engineering:      0 00% clear          (0/34)      Outfitting:      9 09% clear          (0/34)\n         Alchemy:      0 00% clear          (0/34)      Enchanting:      0 00% clear          (0/34)\n     Scholarship:     10 86% clear          (0/34) Mechanical Lore:      2 02% clear          (0/34)\n       Appraisal:     11 58% clear          (0/34)     Performance:      2 00% clear          (0/34)\n         Tactics:      8 41% clear          (0/34)\n\nTotal Ranks Displayed: 384\nTime Development Points: 3  Favors: 0  Deaths: 0  Departs: 0\nOverall state of mind: clear\nEXP HELP for more information\n<output class=\"\"/>";

			string[] stringChunks = null;

			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Pathfinder.Core.Tests.Data.exp-chunks.txt"))
			using (var reader = new StreamReader(stream)) {
				var data = reader.ReadToEnd();
				stringChunks = data.Split(new string[]{ "^---------------------^" }, StringSplitOptions.None);
			}

			var result = theReader.Read(Chunk.For(stringChunks[0]));
			Assert.AreEqual(expected1, result.Chunk.Text);

			result = theReader.Read(Chunk.For(stringChunks[1]));
			Assert.AreEqual(expected2, result.Chunk.Text);

			result = theReader.Read(Chunk.For(stringChunks[2]));
			Assert.AreEqual(expected3, result.Chunk.Text);
		}

		[Test]
		public void handles_entire_chunk()
		{
			const string expected = "<output class=\"mono\"/>\n\nCircle: 3\nShowing all skills with field experience.\n\n          SKILL: Rank/Percent towards next rank/Amount learning/Mindstate Fraction\n       Athletics:     27 97% perusing       (2/34)\n\nTotal Ranks Displayed: 27\nTime Development Points: 3  Favors: 0  Deaths: 0  Departs: 0\nOverall state of mind: clear\nEXP HELP for more information\n<output class=\"\"/>";

			var result = theReader.Read(Chunk.For(expected));
			Assert.AreEqual(expected, result.Chunk.Text);
		}
	}
}
