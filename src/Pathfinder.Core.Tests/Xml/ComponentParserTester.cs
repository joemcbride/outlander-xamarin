using System;
using System.Linq;
using NUnit.Framework;

namespace Pathfinder.Core.Xml.Tests
{
	[TestFixture]
	public class ComponentParserTester
	{
		private ComponentParser theParser;

		private const string SampleData = "<component id='exp Small Blunt'>     Small Blunt:  149 15% captivated   </component>";

		[SetUp]
		public void SetUp()
		{
			theParser = new ComponentParser ();
		}

		[Test]
		public void matches()
		{
			Assert.True(theParser.Matches(SampleData));
		}

		[Test]
		public void sets_the_id()
		{
			var result = theParser.Parse(SampleData).Single();
			Assert.AreEqual ("exp Small Blunt", result.Id);
		}

		[Test]
		public void sets_the_content()
		{
			var result = theParser.Parse(SampleData).Single();
			Assert.AreEqual ("Small Blunt:  149 15% captivated", result.Content);
		}

		[Test]
		public void is_valid_with_id()
		{
			var result = theParser.Parse(SampleData).Single();
			Assert.True(result.IsValid());
		}

		[Test]
		public void is_not_valid_without_id()
		{
			var result = new ComponentResult();
			Assert.False(result.IsValid());
		}

		[Test]
		public void room_desc_test()
		{
			const string roomDescription = "The stony soil forces the tree roots to crawl across the slope like thick-bodied snakes, seeking moisture and purchase in the cracks of the stone.";
			string data = "<component id='room desc'>" + roomDescription + "</component>";
			var result = theParser.Parse(data).Single();
			Assert.AreEqual("room desc", result.Id);
			Assert.AreEqual(roomDescription, result.Content);
		}
	}
}

