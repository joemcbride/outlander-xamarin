using System;
using NUnit.Framework;
using Pathfinder.Core.Text;

namespace Pathfinder.Core.Text.Tests
{
	[TestFixture]
	public class ComponentTagTester
	{
		[Test]
		public void parses_id_for_exp()
		{
			const string data = "<component id='exp Elemental Magic'><preset id='whisper'> Elemental Magic:   12 17% dabbling     </preset></component>";

			var tag = new ComponentTag(data);

			Assert.AreEqual("Elemental_Magic", tag.Id);
			Assert.AreEqual("12 17% dabbling", tag.Value);
		}

		[Test]
		public void parses_value_for_exp()
		{
			const string data = "<component id='exp Sorcery'>      Sorcery:   321 56% learning     </component>";

			var tag = new ComponentTag(data);

			Assert.AreEqual("Sorcery", tag.Id);
			Assert.AreEqual("321 56% learning", tag.Value);
		}
	}
}

