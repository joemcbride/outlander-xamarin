using System.Linq;
using NUnit.Framework;

namespace Outlander.Core.Authentication.Tests
{
    [TestFixture]
	public class AuthGameParserTester
    {
		private AuthGameParser theParser;

        [SetUp]
        public void Setup()
        {
			theParser = new AuthGameParser();
        }

        [Test]
        public void parses_games()
        {
            const string gameData = "M	CS	CyberStrike 	DR	DragonRealms	DRD	DragonRealms Development	DRDGT	DragonRealms Dev GM Training	DRF	DragonRealms The Fallen	DRT	DragonRealms Prime Test	DRX	DragonRealms Platinum	GS3	GemStone IV	GS4D	GemStone IV Development	GSF	GemStone IV Shattered	GST	GemStone IV Prime Test	GSX	GemStone IV Platinum	HX	Alliance of Heroes	HXD	Alliance of Heroes Development	MO	Modus Operandi 	MOD	Modus Operandi Development";

            var games = theParser.Parse(gameData).ToList();

            Assert.AreEqual(16, games.Count);

            Assert.AreEqual("CS", games[0].Code);
            Assert.AreEqual("CyberStrike", games[0].Name);

            Assert.AreEqual("DR", games[1].Code);
            Assert.AreEqual("DragonRealms", games[1].Name);

            Assert.AreEqual("DRD", games[2].Code);
            Assert.AreEqual("DragonRealms Development", games[2].Name);
        }
    }
}
