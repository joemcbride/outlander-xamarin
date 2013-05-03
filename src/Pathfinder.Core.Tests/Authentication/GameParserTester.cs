using System.Linq;
using NUnit.Framework;
using Pathfinder.Core.Authentication;

namespace Pathfinder.Core.Tests.Authentication
{
    [TestFixture]
    public class GameParserTester
    {
        private GameParser theParser;

        [SetUp]
        public void Setup()
        {
            theParser = new GameParser();
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
