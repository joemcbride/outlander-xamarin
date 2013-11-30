using System;
using System.Linq;
using NUnit.Framework;

namespace Pathfinder.Core.Tests.Xml
{
	[TestFixture]
	public class RightHandParserTester
	{
		private const string theData = "<right exist=\"99767547\" noun=\"rucksack\">wool rucksack</right>";

		private RightHandParser theParser;
		private RightHandResult theResult;

		[SetUp]
		public void SetUp()
		{
			theParser = new RightHandParser();
			theResult = theParser.Parse(theData).Single();
		}

		[Test]
		public void sets_the_noun()
		{
			Assert.AreEqual("rucksack", theResult.Noun);
		}

		[Test]
		public void sets_the_name()
		{
			Assert.AreEqual("wool rucksack", theResult.Name);
		}

		[Test]
		public void sets_the_item_id()
		{
			Assert.AreEqual("99767547", theResult.ItemId);
		}
	}

	[TestFixture]
	public class LeftHandParserTester
	{
		private const string theData = "<left exist=\"99737474\" noun=\"scissors\">serrated scissors</left>";

		private LeftHandParser theParser;
		private LeftHandResult theResult;

		[SetUp]
		public void SetUp()
		{
			theParser = new LeftHandParser();
			theResult = theParser.Parse(theData).Single();
		}

		[Test]
		public void sets_the_noun()
		{
			Assert.AreEqual("scissors", theResult.Noun);
		}

		[Test]
		public void sets_the_name()
		{
			Assert.AreEqual("serrated scissors", theResult.Name);
		}

		[Test]
		public void sets_the_item_id()
		{
			Assert.AreEqual("99737474", theResult.ItemId);
		}
	}
}
