using System;
using System.Linq;
using NUnit.Framework;

namespace Pathfinder.Core.Text.Tests
{
	[TestFixture]
	public class RoomNameChunkReaderTester
	{
		[Test]
		public void reads_the_tag()
		{
			const string tag = "<style id=\"roomName\" />[The Western Road, Stream Bank]\n<style id=\"\"/>";

			var reader = new ChunkReader<RoomNameTag>("<style id=\"roomName\"", "<style id=\"\"");
			var result = reader.Read(Chunk.For(tag));

			Assert.AreEqual(tag, result.Tags.Single().Text);
			Assert.AreEqual("[The Western Road, Stream Bank]\n", result.Tags.OfType<RoomNameTag>().Single().Name);
		}
	}
}
