using System;
using NUnit.Framework;
using Outlander.Core.Text;

namespace Outlander.Core.Text.Tests
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
		public void removes_newlines()
		{
			const string data = "\n\nObvious paths: <d>north</d>, <d>east</d>, <d>west</d>.";
			const string expected = "\nObvious paths: <d>north</d>, <d>east</d>, <d>west</d>.";

			Check(data, expected);
		}

		[Test]
		public void handles_exists()
		{
			const string data = "Obvious exits: <d>out</d>.";
			const string expected = "Obvious exits: <d>out</d>.";

			Check(data, expected);
		}

		private void Check(string data, string expected)
		{
			var result = theReader.Read(Chunk.For(data));

			Assert.AreEqual(expected, result.Chunk.Text);
		}
	}
}
