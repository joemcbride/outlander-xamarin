using System;
using System.Linq;
using NUnit.Framework;
using Pathfinder.Core.Tests;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class HighlightsLoaderTester
	{
		private HighlightsLoader theLoader;
		private StubFileSystem theFileSystem;
		private AppSettings theAppSettings;

		[SetUp]
		public void SetUp()
		{
			theFileSystem = new StubFileSystem();
			theAppSettings = new AppSettings();
			theAppSettings.HomeDirectory = "Documents/Outlander";
			theLoader = new HighlightsLoader(theFileSystem);
		}

		[Test]
		public void loads_highlights()
		{
			const string vars = "#highlight {#fff} {^You've gained a new rank.*$}\n#highlight {one} {some value}";

			theFileSystem.StubReadAllText(vars);

			var result = theLoader.Load("something").ToList();

			Assert.AreEqual(2, result.Count);

			Assert.AreEqual("#fff", result[0].Color);
			Assert.AreEqual("^You've gained a new rank.*$", result[0].Pattern);

			Assert.AreEqual("one", result[1].Color);
			Assert.AreEqual("some value", result[1].Pattern);
		}

		[Test]
		public void parses_highlights()
		{
			const string vars = "#highlight {#fff} {^You've gained a new rank.*$}\n#highlight {one} {some value}";

			var result = theLoader.Parse(vars).ToList();

			Assert.AreEqual(2, result.Count);

			Assert.AreEqual("#fff", result[0].Color);
			Assert.AreEqual("^You've gained a new rank.*$", result[0].Pattern);

			Assert.AreEqual("one", result[1].Color);
			Assert.AreEqual("some value", result[1].Pattern);
		}
	}
}

