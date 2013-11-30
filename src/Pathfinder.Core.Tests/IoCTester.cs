using System;
using System.Linq;
using NUnit.Framework;
using Pathfinder.Core;
using Pathfinder.Core.Xml;

namespace Pathfinder.Core.Container.Tests
{
	[TestFixture]
	public class IoCTester
	{
		private SimpleContainer theContainer;

		[SetUp]
		public void SetUp()
		{
			theContainer = new SimpleContainer();
			theContainer.PerRequest<IParser, VitalsParser>();

			IoC.BuildUp = theContainer.BuildUp;
			IoC.GetInstance = theContainer.GetInstance;
			IoC.GetAllInstances = theContainer.GetAllInstances;
		}

		[Test]
		public void should_get_parser()
		{
			var parser = IoC.Get<IParser>();
			Assert.NotNull(parser);
			Assert.AreEqual(typeof(VitalsParser), parser.GetType());
		}

		[Test]
		public void should_get_game_parser()
		{
			theContainer.PerRequest<ITransformer, PopStreamTransformer>();
			theContainer.PerRequest<IParser, PromptParser>();
			theContainer.PerRequest<GameParser>();

			var parser = IoC.Get<GameParser>();
			Assert.NotNull(parser);
		}
	}
}
