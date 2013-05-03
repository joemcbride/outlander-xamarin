using NUnit.Framework;
using Pathfinder.Core.Authentication;

namespace Pathfinder.Core.Tests.Authentication
{
    [TestFixture]
    public class ConnectionTokenParserTester
    {
        private ConnectionTokenParser theParser;

        [SetUp]
        public void Setup()
        {
            theParser = new ConnectionTokenParser();
        }

        [Test]
        public void parses_connection_token()
        {
            const string sampleData =
                "L	OK	UPPORT=5535	GAME=WIZ	GAMECODE=DR	FULLGAMENAME=Wizard Front End	GAMEFILE=WIZARD.EXE	GAMEHOST=dr.simutronics.net	GAMEPORT=4901	KEY=f42cweae9e0ab8c80318a1e7800753e1";

            var token = theParser.Parse(sampleData);

            Assert.AreEqual("dr.simutronics.net", token.GameHost);
            Assert.AreEqual(4901, token.GamePort);
            Assert.AreEqual("f42cweae9e0ab8c80318a1e7800753e1", token.Key);
        }
    }
}
