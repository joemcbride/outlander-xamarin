using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Pathfinder.Core.Tests.Xml
{
	[TestFixture]
	public class VitalsResultTester
	{
		private const string xml = "<dialogData id='minivitals'><skin id='healthSkin' name='healthBar' controls='health' left='0%' top='0%' width='20%' height='100%'/><progressBar id='health' value='100' text='health 100%' left='0%' customText='t' top='0%' width='20%' height='100%'/><skin id='manaSkin' name='manaBar' controls='mana' left='20%' top='0%' width='20%' height='100%'/><progressBar id='mana' value='88' text='mana 88%' left='20%' customText='t' top='0%' width='20%' height='100%'/><skin id='staminaSkin' name='staminaBar' controls='stamina' left='40%' top='0%' width='20%' height='100%'/><progressBar id='stamina' value='99' text='fatigue 99%' left='40%' customText='t' top='0%' width='20%' height='100%'/><progressBar id='concentration' value='98' text='concentration 98%' left='80%' customText='t' top='0%' width='20%' height='100%'/></dialogData>";
		private VitalsResult theResult;

		[SetUp]
		public void SetUp()
		{
			theResult = VitalsResult.For(xml);
		}

		[Test]
		public void sets_matched()
		{
			Assert.AreEqual(xml, theResult.Matched);
		}

		[Test]
		public void sets_name()
		{
			Assert.AreEqual("health", theResult.Name);
		}

		[Test]
		public void sets_value()
		{
			Assert.AreEqual(100, theResult.Value);
		}
	}
}
