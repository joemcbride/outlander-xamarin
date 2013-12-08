using System;
using NUnit.Framework;

namespace Pathfinder.Core.Tests
{
	[TestFixture]
	public class GameServerTester
	{
		private GameServer theGameServer;
		private StubSimpleSocket theSocket;
		private ILog theLogger;

		[SetUp]
		public void SetUp()
		{
			theSocket = new StubSimpleSocket();
			theLogger = new NullLog();
			theGameServer = new GameServer(theSocket, theLogger);
		}

		[Test]
		public void splits_data_into_line_feed_chunks()
		{
			const string stream = "\n<streamWindow id=\"main\" title=\"Story\" location=\"center\" target=\"drop\" resident=\"true\"/>\n<streamWindow id='inv' title='My Inventory' target='wear' ifClosed='' resident='true'/>\n<clear";
			const string stream2 = "Stream id='inv' ifClosed=''/>\n";

			const string expect1 = "\n<streamWindow id=\"main\" title=\"Story\" location=\"center\" target=\"drop\" resident=\"true\"/>\n<streamWindow id='inv' title='My Inventory' target='wear' ifClosed='' resident='true'/>\n";
			const string expect2 = "<clearStream id='inv' ifClosed=''/>\n";

			theSocket.SetRecieveData(stream);

			var data = theGameServer.Poll();

			Assert.AreEqual(expect1, data);

			theSocket.SetRecieveData(stream2);
			data = theGameServer.Poll();

			Assert.AreEqual(expect2, data);
		}
	}

	public class StubSimpleSocket : ISimpleSocket
	{
		private string _receiveData;
		private Exception _lastError = null;

		public void Close()
		{
			throw new NotImplementedException();
		}

		public void Connect(string host, int port)
		{
			throw new NotImplementedException();
		}

		public bool Send(string data)
		{
			throw new NotImplementedException();
		}

		public string SendAndReceive(string data)
		{
			throw new NotImplementedException();
		}

		public string Receive()
		{
			return _receiveData;
		}

		public void Shutdown(System.Net.Sockets.SocketShutdown option)
		{
			throw new NotImplementedException();
		}

		public Exception LastError {
			get {
				return _lastError;
			}
		}

		public bool Connected {
			get {
				return true;
			}
		}

		public void Dispose()
		{
		}

		public void SetRecieveData(string data)
		{
			_receiveData = data;
		}
	}
}
