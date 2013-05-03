using NUnit.Framework;
using Pathfinder.Core.Xml;

namespace Pathfinder.Core.Tests.Xml
{
    [TestFixture]
    public class PromptParserTester
    {
        private PromptParser theParser;
        private const string SampleData = "<prompt time=\"1366780108\">H&gt;</prompt>";

        [SetUp]
        public void Setup()
        {
            theParser = new PromptParser();
        }

        [Test]
        public void should_match_for_prompt()
        {
            Assert.True(theParser.Matches(SampleData));
        }

        [Test]
        public void should_parse_prompt()
        {
            var result = theParser.Parse(SampleData);

            Assert.AreEqual("H>", result);
        }

        [Test]
        public void should_send_a_prompt_event()
        {
            var result = theParser.Parse(SampleData);

            Assert.AreEqual("H>", result);
        }
    }
}
