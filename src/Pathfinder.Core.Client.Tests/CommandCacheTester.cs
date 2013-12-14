using System;
using NUnit.Framework;

namespace Pathfinder.Core.Client.Tests
{
	[TestFixture]
	public class CommandCacheTester
	{
		private CommandCache theCache;

		[SetUp]
		public void SetUp()
		{
			theCache = new CommandCache();
		}

		[Test]
		public void adds_new_command()
		{
			const string command = "my command";

			theCache.Add(command);

			Assert.AreEqual(command, theCache.LastCommand());
			Assert.AreEqual(-1, theCache.CurrentIndex);
		}

		[Test]
		public void removes_command_after_max_is_reached()
		{
			const string command1 = "my command 1";
			const string command2 = "my command 2";
			const string command3 = "my command 3";
			const string command4 = "my command 4";

			theCache.MaxSize = 3;

			theCache.Add(command1);
			theCache.Add(command2);
			theCache.Add(command3);
			theCache.Add(command4);

			Assert.AreEqual(command4, theCache.LastCommand());
			Assert.AreEqual(-1, theCache.CurrentIndex);
			Assert.AreEqual(3, theCache.Count);
		}

		[Test]
		public void removes_excess_commands_when_maxsize_set()
		{
			var commands = new string[] { "one", "two", "three" };

			commands.Apply(theCache.Add);

			theCache.MaxSize = 2;

			Assert.AreEqual(2, theCache.Count);
		}

		[Test]
		public void ignores_commands_less_than_min_length()
		{
			theCache.CommandMinLength = 3;

			theCache.Add("a");
			Assert.AreEqual(0, theCache.Count);

			theCache.Add("ab");
			Assert.AreEqual(0, theCache.Count);

			theCache.Add("abc");
			Assert.AreEqual(1, theCache.Count);
		}

		[Test]
		public void ignores_empty_commands()
		{
			theCache.CommandMinLength = 0;

			theCache.Add(string.Empty);
			Assert.AreEqual(0, theCache.Count);

			theCache.Add(" ");
			Assert.AreEqual(0, theCache.Count);

			theCache.Add(null);
			Assert.AreEqual(0, theCache.Count);
		}

		[Test]
		public void ignores_repeated_commands()
		{
			const string command1 = "my command 1";

			theCache.Add(command1);
			Assert.AreEqual(1, theCache.Count);

			theCache.Add(command1);
			Assert.AreEqual(1, theCache.Count);
		}
	}
}
