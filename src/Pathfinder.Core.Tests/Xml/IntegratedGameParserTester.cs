using System;
using System.Linq;
using NUnit.Framework;
using Pathfinder.Core.Xml;

namespace Pathfinder.Core.Tests
{
	[TestFixture]
	public class IntegratedGameParserTester
	{
		private GameParser theParser;
		private GameData theGameData;

		[SetUp]
		public void SetUp()
		{
			theParser = new GameParser(
				new ITransformer[] {
					new PopStreamTransformer()
				},
				new IParser[] {
					new ComponentParser(),
					new RoundtimeParser(),
					new GenericStreamParser(),
					new CompassParser()
				});
			theGameData = new GameData();
		}

		[Test]
		public void parses_items()
		{
			const string streamXml = "<pushStream id=\"thoughts\"/><preset id='thought'>Your mind hears Mimaway thinking, </preset>\"The quality isn't good enough, any idea what I might be doing wrong? Or maybe I'm not ready for challenging work orders yet.\"\n<popStream/><pushStream id=\"logons\"/> * Criminal Mastermind Jalika Tenretni snuck out of the shadow she was hiding in.\r<popStream/>";
			const string compassXml = "<compass><dir value=\"ne\"/><dir value=\"sw\"/></compass>";
			const string text = "\nWith skill and nimble fingers you avoid the centipede that runs along in front of your fingers.\nYou manage to collect a pile of rocks.\nRoundtime: 15 sec.";
			const string text2 = "\nYou take a step back and run up to the pile of rocks.  Bringing your foot smashing down onto the pile, you send bits and pieces of debris scattering in every direction.  Looking around for the now defunct mountain of glory, you fail to find any signs of its presence.\n";

			theGameData.Append(text);
			theGameData.Append(streamXml);
			theGameData.Append("<component id='exp Small Blunt'>     Small Blunt:  149 15% captivated   </component>");
			theGameData.Append("<component id='exp Small Blunt'>     Small Blunt:  147 15% captivated   </component>");
			theGameData.Append("<roundTime value='1366812703'/>");
			theGameData.Append(text2);
			theGameData.Append(compassXml);
			var results = theParser.Parse(theGameData).ToList();
			Assert.AreEqual(7, results.Count);
			Assert.AreEqual(text + text2, theGameData.Current);
		}
	}
}