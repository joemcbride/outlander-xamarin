using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Outlander.Core.Text;

namespace Outlander.Core.Text.Tests
{	

	[TestFixture]
	public class PromptChunkReaderTester
	{
		private ChunkReader<Tag> reader;

		[SetUp]
		public void SetUp()
		{
			reader = new ChunkReader<Tag>("<prompt", "</prompt");
		}

		[Test]
		public void reads_prompt()
		{
			const string promptTag = "<prompt time=\"1385828459\">H&gt;</prompt>";

			var result = reader.Read(Chunk.For(promptTag));
			Assert.AreEqual(1, result.Tags.Count());
			Assert.AreEqual(promptTag, result.Tags.Single().Text);
		}

		[Test]
		public void reads_prompt_between_tags()
		{
			const string promptTag = "<prompt time=\"1385828459\">H&gt;</prompt>";
			const string compassTag = "<compass><dir value=\"e\"/><dir value=\"nw\"/></compass>";

			var result = reader.Read(Chunk.For(compassTag + promptTag + compassTag));
			Assert.AreEqual(1, result.Tags.Count());
			Assert.AreEqual(promptTag, result.Tags.Single().Text);
		}

		[Test]
		public void extracts_data_between_chunks()
		{
			const string tag = "<compass><dir value=\"e\"/><dir value=\"nw\"/></compass><prompt ";
			const string tag2 = "time=\"1385828459\">H&gt;</prompt>";
			const string expected = "<prompt time=\"1385828459\">H&gt;</prompt>";

			var result = reader.Read(Chunk.For(tag));
			Assert.AreEqual(0, result.Tags.Count());
			Assert.AreEqual("<compass><dir value=\"e\"/><dir value=\"nw\"/></compass>", result.Chunk.Text);

			result = reader.Read(Chunk.For(tag2));
			Assert.AreEqual(1, result.Tags.Count());
			Assert.AreEqual(expected, result.Tags.Single().Text);
			Assert.Null(result.Chunk);
		}

		[Test]
		public void extracts_data_between_chunks_2()
		{
			const string tag = "<compass><dir value=\"e\"/><dir value=\"nw\"/></compass><prompt time=\"13858";
			const string tag2 = "28459\">H&gt;</prompt>";
			const string expected = "<prompt time=\"1385828459\">H&gt;</prompt>";

			var result = reader.Read(Chunk.For(tag));
			Assert.AreEqual(0, result.Tags.Count());
			Assert.AreEqual("<compass><dir value=\"e\"/><dir value=\"nw\"/></compass>", result.Chunk.Text);

			result = reader.Read(Chunk.For(tag2));
			Assert.AreEqual(1, result.Tags.Count());
			Assert.AreEqual(expected, result.Tags.Single().Text);
			Assert.Null(result.Chunk);
		}
	}
}
