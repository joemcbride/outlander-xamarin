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
			theGameState.Set(ComponentKeys.RoomTitle, title);

			var tags = theHighlighter.For(TextTag.For(title + "\n" + description + "\n" + roomObjs)).ToList();

			Assert.AreEqual(4, tags.Count);
			Assert.AreEqual(TextTag.For(title), tags[0]);
			Assert.AreEqual("#0000FF", tags[0].Color);
			Assert.AreEqual(TextTag.For("\n" + description + "\nYou also see "), tags[1]);
			Assert.AreEqual("a musk hog", tags[2].Text);
			Assert.AreEqual("#FFFF00", tags[2].Color);
		}
	}
}
