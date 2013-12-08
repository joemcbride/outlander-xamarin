using System;
using NUnit.Framework;
using Pathfinder.Core.Text;

namespace Pathfinder.Core.Text.Tests
{
	[TestFixture]
	public class ObivousPathsChunkReaderTester
	{
		private ObivousPathsChunkReader theReader;

		[SetUp]
		public void SetUp()
		{
			theReader = new ObivousPathsChunkReader();
		}

		[Test]
		public void adds_newline_if_none()
		{
			const string data = "Obvious paths: <d>north</d>, <d>east</d>, <d>west</d>.";
			const string expected = "\nObvious paths: <d>north</d>, <d>east</d>, <d>west</d>.";

			Check(data, expected);
		}

		[Test]
		public void handles_exists()
		{
			const string data = "Obvious exits: <d>out</d>.";
			const string expected = "\nObvious exits: <d>out</d>.";

			Check(data, expected);
		}

		private void Check(string data, string expected)
		{
			var result = theReader.Read(Chunk.For(data));

			Assert.AreEqual(expected, result.Chunk.Text);
		}
	}
}
