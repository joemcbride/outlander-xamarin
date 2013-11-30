using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Pathfinder.Core.Xml.Tests
{
	[TestFixture]
	public class CompassParserTester
	{
		private const string xml = "<compass><dir value=\"ne\"/><dir value=\"sw\"/></compass>";

		private CompassParser theParser;
		private List<CompassResult> theResults;

		[SetUp]
		public void SetUp()
		{
			theParser = new CompassParser();
			theResults = theParser.Parse(xml).ToList();
		}

		[Test]
		public void parses_ne()
		{
			Assert.AreEqual("ne", theResults[0].Direction);
		}

		[Test]
		public void parses_sw()
		{
			Assert.AreEqual("sw", theResults[1].Direction);
		}

		[Test]
		public void sets_matched()
		{
			Assert.AreEqual(xml, theResults[0].Matched);
		}
	}
}
