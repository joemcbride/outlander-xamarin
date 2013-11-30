using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Pathfinder.Core.Tests.Xml
{
	[TestFixture]
	public class VitalsParserTester
	{
		private const string xml = "<dialogData id='minivitals'><skin id='healthSkin' name='healthBar' controls='health' left='0%' top='0%' width='20%' height='100%'/><progressBar id='health' value='100' text='health 100%' left='0%' customText='t' top='0%' width='20%' height='100%'/></dialogData>\n<dialogData id='minivitals'><skin id='manaSkin' name='manaBar' controls='mana' left='20%' top='0%' width='20%' height='100%'/><progressBar id='mana' value='88' text='mana 88%' left='20%' customText='t' top='0%' width='20%' height='100%'/></dialogData>\n<dialogData id='minivitals'><skin id='staminaSkin' name='staminaBar' controls='stamina' left='40%' top='0%' width='20%' height='100%'/><progressBar id='stamina' value='99' text='fatigue 99%' left='40%' customText='t' top='0%' width='20%' height='100%'/></dialogData>\n<dialogData id='minivitals'><progressBar id='concentration' value='98' text='concentration 98%' left='80%' customText='t' top='0%' width='20%' height='100%'/></dialogData>";

		private VitalsParser theParser;
		private List<VitalsResult> theResults;

		[SetUp]
		public void SetUp()
		{
			theParser = new VitalsParser();
			theResults = theParser.Parse(xml).ToList();
		}

		[Test]
		public void parses_health()
		{
			var result = theResults[0];
			Assert.AreEqual("health", result.Name);
			Assert.AreEqual(100, result.Value);
		}

		[Test]
		public void parses_mana()
		{
			var result = theResults[1];
			Assert.AreEqual("mana", result.Name);
			Assert.AreEqual(88, result.Value);
		}

		[Test]
		public void parses_stamina()
		{
			var result = theResults[2];
			Assert.AreEqual("stamina", result.Name);
			Assert.AreEqual(99, result.Value);
		}

		[Test]
		public void parses_concentration()
		{
			var result = theResults[3];
			Assert.AreEqual("concentration", result.Name);
			Assert.AreEqual(98, result.Value);
		}
	}
}
