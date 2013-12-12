using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Pathfinder.Core.Tests;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class IntegratedHighlightsTester
	{
		private IGameState theGameState;
		private HighlightSettings theSettings;
		private Highlights theHighlighter;

		[SetUp]
		public void SetUp()
		{
			theGameState = new StubGameState();
			theSettings = HighlightSettings.Default();

			theHighlighter = new Highlights(new IHighlighter[] {
				new BoldHighlighter(theSettings),
				new RoomNameHighlighter(theGameState, theSettings)
			});
		}

		[Test]
		public void highlights_room_name_and_bold()
		{
			const string title = "[Derelict Road, Darkling Wood]";
			const string description = "The stony soil forces the tree roots to crawl across the slope like thick-bodied snakes, seeking moisture and purchase in the cracks of the stone.  To the south, the forest thins as the soil yields further to the stone of the mountains that lurk just beyond.";
			const string roomObjs = "You also see <pushBold/>a musk hog<popBold/>.";

			theGameState.Set(ComponentKeys.RoomName, title);

			var tags = theHighlighter.For(title + "\n" + description + "\n" + roomObjs).ToList();

			Assert.AreEqual(4, tags.Count);
			Assert.AreEqual(TextTag.For(title), tags[0]);
			Assert.AreEqual("#5858FA", tags[0].Color);
			Assert.AreEqual(TextTag.For(description), tags[1]);
			Assert.AreEqual("a musk hog", tags[2].Text);
			Assert.AreEqual("#FFFF00", tags[2].Color);
		}
	}

	[TestFixture]
	public class RoomNameHighlightsTester
	{
		private IGameState theGameState;
		private HighlightSettings theSettings;
		private Highlights theHighlighter;

		[SetUp]
		public void SetUp()
		{
			theGameState = new StubGameState();
			theSettings = HighlightSettings.Default();

			theHighlighter = new Highlights(new IHighlighter[]{ new RoomNameHighlighter(theGameState, theSettings) });
		}

		[Test]
		public void highlights_room_name()
		{
			const string title = "[Derelict Road, Darkling Wood]";
			const string description = "The stony soil forces the tree roots to crawl across the slope like thick-bodied snakes, seeking moisture and purchase in the cracks of the stone.  To the south, the forest thins as the soil yields further to the stone of the mountains that lurk just beyond.";

			theGameState.Set(ComponentKeys.RoomName, title);

			var tags = theHighlighter.For(title + "\n" + description).ToList();

			Assert.AreEqual(2, tags.Count);
			Assert.AreEqual(TextTag.For(title), tags[0]);
			Assert.AreEqual("#5858FA", tags[0].Color);
			Assert.AreEqual(TextTag.For(description), tags[1]);
		}

		[Test]
		public void highlights_room_name_between_text()
		{
			const string title = "[Derelict Road, Darkling Wood]";
			const string description = "The stony soil forces the tree roots to crawl across the slope like thick-bodied snakes, seeking moisture and purchase in the cracks of the stone.  To the south, the forest thins as the soil yields further to the stone of the mountains that lurk just beyond.";

			theGameState.Set(ComponentKeys.RoomName, title);

			var tags = theHighlighter.For(description + "\n\n" + title + "\n" + description).ToList();

			Assert.AreEqual(3, tags.Count);
			Assert.AreEqual(TextTag.For(description), tags[0]);
			Assert.AreEqual(TextTag.For(title), tags[1]);
			Assert.AreEqual(TextTag.For(description), tags[2]);
		}
	}

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

			var tags = theHighlighter.For(title).ToList();

			Assert.AreEqual(1, tags.Count);
			Assert.AreEqual("a musk hog", tags[0].Text);
			Assert.AreEqual("#FFFF00", tags[0].Color);
		}

		[Test]
		public void highlights_monster_bold_multiple_monsters()
		{
			const string title = "You also see <pushBold/>a musk hog<popBold/>, <pushBold/>a musk hog<popBold/> and <pushBold/>a musk hog<popBold/>.";

			var tags = theHighlighter.For(title).ToList();

			Assert.AreEqual(7, tags.Count);
			Assert.AreEqual("You also see ", tags[0].Text);
			Assert.AreEqual("a musk hog", tags[1].Text);
			Assert.AreEqual("#FFFF00", tags[1].Color);
		}

		[Test]
		public void something()
		{
			const string data = "<pushBold/>*********************IMPORTANT!***********************\n<popBold/>Closing the STORMFRONT front end does NOT necessarily mean your\nCharacter will be dropped from the game!  To ensure\nthat your character is disconnected type QUIT or EXIT!\n<pushBold/>*********************IMPORTANT!***********************\n\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\n<popBold/>Dragons of Elanthia just went into Alpha Testing!  Sign up\nfor the beta at http://www.dragonsofelanthia.com and keep\nyour eyes peeled for an Alpha Key in your email!\n\nIf you don't have an Alpha Key yet, you can still watch the\naction on http://www.twitch.tv/simutronics!\n<pushBold/>* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\n<popBold/>";

			var tags = theHighlighter.For(data).ToList();

			Assert.AreEqual(5, tags.Count);
		}
	}
}
