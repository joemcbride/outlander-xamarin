using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Pathfinder.Core.Tests;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class BoldHighligtherTester
	{
		private HighlightSettings theSettings;
		private Highlights theHighlighter;

		[SetUp]
		public void SetUp()
		{
			theSettings = HighlightSettings.Default();

			theHighlighter = new Highlights(new IHighlighter[]{ new BoldHighlighter(theSettings) });
		}

		[Test]
		public void highlights_monster_bold()
		{
			const string title = "<pushBold/>a musk hog<popBold/>";

			var tags = theHighlighter.For(TextTag.For(title)).ToList();

			Assert.AreEqual(1, tags.Count);
			Assert.AreEqual("a musk hog", tags[0].Text);
			Assert.AreEqual("#FFFF00", tags[0].Color);
		}

		[Test]
		public void highlights_monster_bold_multiple_monsters()
		{
			const string title = "You also see <pushBold/>a musk hog<popBold/>, <pushBold/>a musk hog<popBold/> and <pushBold/>a musk hog<popBold/>.";

			var tags = theHighlighter.For(TextTag.For(title)).ToList();

			Assert.AreEqual(7, tags.Count);
			Assert.AreEqual("You also see ", tags[0].Text);
			Assert.AreEqual("a musk hog", tags[1].Text);
			Assert.AreEqual("#FFFF00", tags[1].Color);
		}

		[Test]
		public void other_bold()
		{
			const string data = "<pushBold/>*********************IMPORTANT!***********************\n<popBold/>Closing the STORMFRONT front end does NOT necessarily mean your\nCharacter will be dropped from the game!  To ensure\nthat your character is disconnected type QUIT or EXIT!\n<pushBold/>*********************IMPORTANT!***********************\n\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\n<popBold/>Dragons of Elanthia just went into Alpha Testing!  Sign up\nfor the beta at http://www.dragonsofelanthia.com and keep\nyour eyes peeled for an Alpha Key in your email!\n\nIf you don't have an Alpha Key yet, you can still watch the\naction on http://www.twitch.tv/simutronics!\n<pushBold/>* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\n<popBold/>";

			var tags = theHighlighter.For(TextTag.For(data)).ToList();

			Assert.AreEqual(5, tags.Count);
		}
	}
}
