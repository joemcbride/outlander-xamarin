using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Pathfinder.Core.Tests;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class VariablesLoaderTester
	{
		private VariablesLoader theLoader;
		private StubFileSystem theFileSystem;
		private AppSettings theAppSettings;

		[SetUp]
		public void SetUp()
		{
			theFileSystem = new StubFileSystem();
			theAppSettings = new AppSettings();
			theAppSettings.HomeDirectory = "Documents/Outlander";
			theLoader = new VariablesLoader(theFileSystem, theAppSettings);
		}

		[Test]
		public void creates_save_path()
		{
			var vars = new SimpleDictionary<string, string>();

			theLoader.Save(vars.Values(), "vars.config");

			Assert.AreEqual("Documents/Outlander/vars.config", theFileSystem.LastSavePath);
		}

		[Test]
		public void creates_save_contents()
		{
			var vars = new SimpleDictionary<string, string>();
			vars.Set("one", "two");
			vars.Set("three", "four five");

			theLoader.Save(vars.Values(), "vars.config");

			Assert.AreEqual("#var {one} {two}\n#var {three} {four five}\n", theFileSystem.LastSaveContent);
		}

		[Test]
		public void loads_vars()
		{
			const string vars = "#var {key} {value}\n#var {one} {some value}";

			theFileSystem.StubReadAllText(vars);

			var result = theLoader.Load("something").ToList();

			Assert.AreEqual(2, result.Count);

			Assert.AreEqual("key", result[0].Key);
			Assert.AreEqual("value", result[0].Value);

			Assert.AreEqual("one", result[1].Key);
			Assert.AreEqual("some value", result[1].Value);
		}

		[Test]
		public void parses_vars()
		{
			const string vars = "#var {key} {value}\n#var {one} {some value}";

			var result = theLoader.Parse(vars).ToList();

			Assert.AreEqual(2, result.Count);

			Assert.AreEqual("key", result[0].Key);
			Assert.AreEqual("value", result[0].Value);

			Assert.AreEqual("one", result[1].Key);
			Assert.AreEqual("some value", result[1].Value);
		}
	}
}
