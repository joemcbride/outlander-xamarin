using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pathfinder.Core.Text;

namespace Pathfinder.Core.Text.Tests
{	
	[TestFixture]
	public class NewGameParserTester
	{
		private NewGameParser theParser;

		[SetUp]
		public void SetUp()
		{
			theParser = new NewGameParser();
		}

		[Test]
		public void parses()
		{
			const string compassTag = "<compass><dir value=\"e\"/><dir value=\"nw\"/></compass>";
			const string streamTag = "<pushStream id=\"logons\"/> * Force of Nature Saracus Soulhawk snuck out of the shadow he was hiding in.\n<popStream/>";
			const string promptTag = "<prompt time=\"1385828459\">H&gt;</prompt>";
			const string dialogTag = "<dialogData id='minivitals'><skin id='healthSkin' name='healthBar' controls='health' left='0%' top='0%' width='20%' height='100%'/><progressBar id='health' value='100' text='health 100%' left='0%' customText='t' top='0%' width='20%' height='100%'/></dialogData>";
			const string roundtimeTag = "<roundTime value='1366812703'/>";
			const string leftTag = "<left>Empty</left>";
			const string rightTag = "<right>Empty</right>";

			var chunk = Chunk.For(compassTag + streamTag + roundtimeTag + promptTag + leftTag + rightTag + dialogTag);

			var result = theParser.Parse(chunk);

			var results = result.Tags.ToList();

			Assert.AreEqual(7, results.Count);
			Assert.AreEqual(streamTag, results[0].Text);
			Assert.AreEqual(promptTag, results[1].Text);
			Assert.AreEqual(dialogTag, results[2].Text);
			Assert.AreEqual(compassTag, results[3].Text);
			Assert.AreEqual(roundtimeTag, results[4].Text);
			Assert.AreEqual(leftTag, results[5].Text);
			Assert.AreEqual(rightTag, results[6].Text);
		}
	}
}
