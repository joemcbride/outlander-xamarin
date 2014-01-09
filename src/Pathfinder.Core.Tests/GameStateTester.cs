using System;
using System.Linq;
using NUnit.Framework;
using Outlander.Core.Text;

namespace Outlander.Core.Tests
{
	[TestFixture]
	public class GameStateTester
	{
		private SimpleGameState theGameState;
		private InMemoryServiceLocator theServices;

		[SetUp]
		public void SetUp()
		{
			theServices = new InMemoryServiceLocator();

			theGameState = new SimpleGameState(new NewGameParser(Enumerable.Empty<ITagTransformer>()), new RoundtimeHandler(theServices));

			theServices.Add<IGameState>(theGameState);
		}

		[Test]
		public void sets_monster_count()
		{
			const string data = "<component id='room objs'>You also see <pushBold/>a zombie stomper<popBold/> and <pushBold/>a zombie stomper<popBold/>.</component>";

			theGameState.Read(data);

			Assert.AreEqual("2", theGameState.Get(ComponentKeys.MonsterCount));
		}

		[Test]
		public void sets_monster_list()
		{
			const string data = "<component id='room objs'>You also see <pushBold/>a zombie stomper<popBold/> and <pushBold/>a zombie stomper<popBold/>.</component>";

			theGameState.Read(data);

			Assert.AreEqual("a zombie stomper, a zombie stomper", theGameState.Get(ComponentKeys.MonsterList));
		}
	}
}
