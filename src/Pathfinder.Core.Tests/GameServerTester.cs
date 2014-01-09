using System;
using System.Collections.Generic;
using NUnit.Framework;
using Outlander.Core.Authentication;
using Outlander.Core.Text;

namespace Outlander.Core.Tests
{
	[TestFixture]
	public class GameServerTester
	{
		private IGameServer theGameServer;
		private StubGameState theGameState;
		private InMemoryServiceLocator theLocator;
		private StubAsyncSocket theSocket;
		private ILog theLogger;

		[SetUp]
		public void SetUp()
		{
			theGameState = new StubGameState();
			theLocator = new InMemoryServiceLocator();
			theLogger = new NullLog();

			theSocket = new StubAsyncSocket();

			theLocator.Add<IAsyncSocket>(theSocket);

			theGameServer = new SimpleGameServer(theGameState, theLogger, theLocator);
		}

		[Test]
		public void splits_data_into_line_feed_chunks()
		{
			const string stream = "\n<streamWindow id=\"main\" title=\"Story\" location=\"center\" target=\"drop\" resident=\"true\"/>\n<streamWindow id='inv' title='My Inventory' target='wear' ifClosed='' resident='true'/>\n<clear";
			const string stream2 = "Stream id='inv' ifClosed=''/>\n";

			const string expect1 = "\n<streamWindow id=\"main\" title=\"Story\" location=\"center\" target=\"drop\" resident=\"true\"/>\n<streamWindow id='inv' title='My Inventory' target='wear' ifClosed='' resident='true'/>\n";
			const string expect2 = "<clearStream id='inv' ifClosed=''/>\n";

			var token = new ConnectionToken();
			token.Key = "abcd1234";

			theGameServer.Connect(token);

			theSocket.FireReceiveMessage(stream);

			Assert.AreEqual(expect1, theGameState.LastReadData);

			theSocket.FireReceiveMessage(stream2);

			Assert.AreEqual(expect2, theGameState.LastReadData);
		}
	}
}
