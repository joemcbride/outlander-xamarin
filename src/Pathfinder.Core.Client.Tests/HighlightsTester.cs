using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class HighlightsTester
	{
		private Highlights theHighlights;
		private List<IHighlighter> theHighlighters;

		[SetUp]
		public void SetUp()
		{
			theHighlighters = new List<IHighlighter>();
			theHighlights = new Highlights(theHighlighters);
		}

		[Test]
		public void keeps_spaces_when_highligting()
		{
			const string data = "* Legendary Ranger Saracus Soulhawk snuck out of the shadow he was hiding in.";

			theHighlighters.Add(SimpleHighlighter.For("Legendary", ""));
			theHighlighters.Add(SimpleHighlighter.For("Ranger", ""));
			theHighlighters.Add(SimpleHighlighter.For("Saracus", ""));

			var result = theHighlights.For(data).ToList();

			Assert.AreEqual(7, result.Count);
		}
	}
}
