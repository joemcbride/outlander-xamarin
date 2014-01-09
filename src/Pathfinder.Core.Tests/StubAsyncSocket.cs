using System;
using System.Collections.Generic;
using NUnit.Framework;
using Outlander.Core.Authentication;
using Outlander.Core.Text;

namespace Outlander.Core.Tests
{

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
	
}
