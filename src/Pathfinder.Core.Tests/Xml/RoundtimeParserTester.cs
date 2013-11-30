using System.Linq;
using NUnit.Framework;
using Pathfinder.Core;
using Pathfinder.Core.Xml;

namespace Pathfinder.Core.Tests.Xml
{
    [TestFixture]
	public class RoundtimeParserTester
    {
		private RoundtimeParser theParser;
		private const string SampleData = "<roundTime value='1366812703'/>";

        [SetUp]
        public void Setup()
        {
			theParser = new RoundtimeParser();
        }

        [Test]
        public void should_match_for_prompt()
        {
			Assert.True(theParser.Matches(SampleData));
        }

        [Test]
		public void should_parse_time()
        {
			var result = theParser.Parse(SampleData).Single();

			Assert.AreEqual("1366812703".UnixTimeStampToDateTime(), result.Finished);
        }

		[Test]
		public void should_set_matched()
		{
			var result = theParser.Parse(SampleData).Single();

			Assert.AreEqual(SampleData, result.Matched);
		}

		[Test]
		public void is_valid()
		{
			var result = theParser.Parse(SampleData).Single();
			Assert.True(result.IsValid());
		}

		[Test]
		public void is_not_valid()
		{
			var result = new RoundtimeResult();
			Assert.False(result.IsValid());
		}
    }
}
