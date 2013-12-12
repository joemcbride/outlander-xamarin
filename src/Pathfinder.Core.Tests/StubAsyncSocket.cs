using System;
using System.Collections.Generic;
using NUnit.Framework;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;

namespace Pathfinder.Core.Tests
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
