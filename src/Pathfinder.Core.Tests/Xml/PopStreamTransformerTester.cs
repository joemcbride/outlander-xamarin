using System;
using NUnit.Framework;

namespace Pathfinder.Core.Tests.Xml
{
	[TestFixture]
	public class PopStreamTransformerTester
	{

		[Test]
		public void transforms_the_stream()
		{
			const string Xml = "<pushStream id=\"thoughts\"/><preset id='thought'>Your mind hears Mimaway thinking, </preset>\"The quality isn't good enough, any idea what I might be doing wrong? Or maybe I'm not ready for challenging work orders yet.\"\n<popStream/>";
			const string Transformed_Xml = "<pushStream id=\"thoughts\"><preset id='thought'>Your mind hears Mimaway thinking, </preset>\"The quality isn't good enough, any idea what I might be doing wrong? Or maybe I'm not ready for challenging work orders yet.\"\n</pushStream>";

			var transformer = new PopStreamTransformer();
			Assert.AreEqual(Transformed_Xml, transformer.Transform(Xml));
		}

		[Test]
		public void transforms_multiple_streams()
		{
			const string Xml = "<pushStream id=\"thoughts\"/><preset id='thought'>Your mind hears Mimaway thinking, </preset>\"The quality isn't good enough, any idea what I might be doing wrong? Or maybe I'm not ready for challenging work orders yet.\"\n<popStream/><pushStream id=\"logons\"/> * Criminal Mastermind Jalika Tenretni snuck out of the shadow she was hiding in.\r<popStream/>";
			const string Transformed_Xml = "<pushStream id=\"thoughts\"><preset id='thought'>Your mind hears Mimaway thinking, </preset>\"The quality isn't good enough, any idea what I might be doing wrong? Or maybe I'm not ready for challenging work orders yet.\"\n</pushStream><pushStream id=\"logons\"> * Criminal Mastermind Jalika Tenretni snuck out of the shadow she was hiding in.\r</pushStream>";

			var transformer = new PopStreamTransformer();
			Assert.AreEqual(Transformed_Xml, transformer.Transform(Xml));
		}
	}
}
