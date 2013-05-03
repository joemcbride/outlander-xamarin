using Moq;
using NUnit.Framework;
using Pathfinder.Core.Events;
using Pathfinder.Core.Xml;

namespace Pathfinder.Core.Tests.Xml
{
    [TestFixture]
    public class PromptParserTester
    {
        private PromptParser theParser;
        private Moq.Mock<IEventAggregator> theEventAggregatorMock;
        private const string SampleData = "<prompt time=\"1366780108\">H&gt;</prompt>";

        [SetUp]
        public void Setup()
        {
            theEventAggregatorMock = new Moq.Mock<IEventAggregator>();
            theParser = new PromptParser(theEventAggregatorMock.Object);
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
            //theEventAggregatorMock.Setup(ag=>ag.Publish(It.IsAny<PromptEvent>()))

            var result = theParser.Parse(SampleData);

            Assert.AreEqual("H>", result);
        }
    }
}
