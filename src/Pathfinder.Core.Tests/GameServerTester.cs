using System;
using NUnit.Framework;
using System.Collections.Generic;
using Pathfinder.Core.Authentication;

namespace Pathfinder.Core.Tests
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

	public class InMemoryServiceLocator : IServiceLocator
	{
		private IDictionary<Type, object> _services = new Dictionary<Type, object>();

		public void Add<T>(T service)
		{
			_services.Add(typeof(T), service);
		}

		public T Get<T>()
		{
			return (T)_services[typeof(T)];
		}

		public IEnumerable<T> GetAll<T>()
		{
			throw new NotImplementedException();
		}
	}

	public class StubGameState : ISimpleGameState
	{
		public string LastReadData { get; set; }

		public string Get(string key)
		{
			return string.Empty;
		}

		public void Set(string key, string value)
		{
		}

		public void Read(string data)
		{
			LastReadData = data;
		}

		public Action<string> TextLog { get; set; }
	}

	public class StubAsyncSocket : IAsyncSocket
	{
		public event ReceiveMessageEventHandler ReceiveMessage;

		public void Connect(string address, int port)
		{
		}

		public void Disconnect()
		{
		}

		public void SendMessage(string message)
		{
		}

		public void FireReceiveMessage(string message)
		{
			var ev = ReceiveMessage;
			if (ev != null)
				ev(message);
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
