using System;
using System.Collections.Generic;
using NUnit.Framework;
using Outlander.Core.Authentication;
using Outlander.Core.Text;

namespace Outlander.Core.Tests
{

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
