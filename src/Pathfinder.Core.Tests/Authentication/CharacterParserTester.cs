using System;
using System.Linq;
using NUnit.Framework;
using Pathfinder.Core.Authentication;

namespace Pathfinder.Core.Tests.Authentication
{
    [TestFixture]
    public class CharacterParserTester
    {
        private CharacterParser theParser;

        [SetUp]
        public void Setup()
        {
            theParser = new CharacterParser();
        }

        [Test]
        public void parses_account_characters()
        {
            const string sample =
                "C	3	16	1	1	W_ACCOUNT_000	Char1	W_ACCOUNT_001	Char2	W_ACCOUNT_002	Char3";

            var characters = theParser.Parse(sample).ToList();

            characters.Select(c => c.CharacterId + ", " + c.Name)
                .Apply(Console.WriteLine);

            Assert.AreEqual("W_ACCOUNT_000", characters[0].CharacterId);
            Assert.AreEqual("Char1", characters[0].Name);

            Assert.AreEqual("W_ACCOUNT_001", characters[1].CharacterId);
            Assert.AreEqual("Char2", characters[1].Name);

            Assert.AreEqual("W_ACCOUNT_002", characters[2].CharacterId);
            Assert.AreEqual("Char3", characters[2].Name);

            Assert.AreEqual(3, characters.Count);
        }
    }
}
