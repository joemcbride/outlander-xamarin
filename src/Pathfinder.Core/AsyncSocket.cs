using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Pathfinder.Core
{
	public class StateObject
	{
		private Guid ID = Guid.NewGuid();
		// Client socket.
		public Socket workSocket = null;
		// Size of receive buffer.
		public const int BufferSize = 1024;
		// Receive buffer.
		public byte[] buffer = new byte[BufferSize];
	}

	public delegate void ReceiveMessageEventHandler(string message);

	public interface IAsyncSocket
	{
		void Connect(string address, int port);
		void Disconnect();
		void SendMessage(string message);

		event ReceiveMessageEventHandler ReceiveMessage;
	}

	public class AsyncSocket : IAsyncSocket
	{
		private ILog _log;

		private ManualResetEvent connectDone = new ManualResetEvent(false);
		private ManualResetEvent sendDone = new ManualResetEvent(false);
		private ManualResetEvent receiveDone = new ManualResetEvent(false);

		private Socket _client = null;
		private IPEndPoint _endpoint = null;

		public event ReceiveMessageEventHandler ReceiveMessage;

		public AsyncSocket()
			: this(new NullLog())
		{
		}

		public AsyncSocket(ILog log)
		{
			_log = log;
		}

		public static IPAddress GetIPAddress(string address)
		{
			IPAddress ipAddress = null;

			if (IPAddress.TryParse(address, out ipAddress))
			{
				return ipAddress;
			}
			else
			{
				IPHostEntry ipHostInfo = Dns.GetHostEntry(address);
				return ipHostInfo.AddressList[ipHostInfo.AddressList.Length - 1];
			}
		}

		private void ConnectCallback(IAsyncResult ar)
		{
			Socket client = (Socket)ar.AsyncState;

			// Complete the connection.
			client.EndConnect(ar);

			_log.Info(string.Format("Socket connected to {0}", client.RemoteEndPoint.ToString()));

			// Signal that the connection has been made.
			connectDone.Set();
		}

		private void Receive()
		{
			var state = new StateObject();
			state.workSocket = _client;

			// Begin receiving the data from the remote device.
			_client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
		}

		private void ReceiveCallback(IAsyncResult ar)
		{
			var state = (StateObject)ar.AsyncState;
			var client = state.workSocket;

			if (client == null || !client.Connected)
				return;

			try
			{
				// Read data from the remote server
				int bytesRead = client.EndReceive(ar);

				if (bytesRead > 0) {

					ReceivedNewMessage(Encoding.Default.GetString(state.buffer, 0, bytesRead));

					// Check for more data
					client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
				} else {
					// Signal that all bytes have been received.
					receiveDone.Set();
				}
			}
			catch (Exception exc) {
				_log.Error(exc);
				receiveDone.Set();
			}
		}

		private void SendCallback(IAsyncResult ar)
		{
			var client = (Socket)ar.AsyncState;

			//int bytesSent = client.EndSend(ar);
			//_log.Info("Sent {0} bytes to server.", bytesSent);

			// Complete sending the data to the remote server
			client.EndSend(ar);

			// Signal that all bytes have been sent.
			sendDone.Set();
		}

		public void SendMessage(string message)
		{
			SendMessage(Encoding.UTF8.GetBytes(message));
		}

		public void SendMessage(byte[] message)
		{
			_client.BeginSend(message, 0, message.Length, 0, new AsyncCallback(SendCallback), _client);
			sendDone.WaitOne();
		}

		public void Connect(string address, int port)
		{
			if (IsConnected)
				Disconnect();

			_endpoint = new IPEndPoint(GetIPAddress(address), port);

			_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_client.BeginConnect(_endpoint, new AsyncCallback(ConnectCallback), _client);
			connectDone.WaitOne();

			Receive();
		}

		public void Disconnect()
		{
			try
			{
				if (_client != null && _client.Connected)
				{
					_client.Shutdown(SocketShutdown.Both);
					_client.Close();
				}
			}
			catch(SocketException exc)
			{
				_log.Error(exc);
			}
			finally
			{
				_client = null;

				connectDone.Reset();
				sendDone.Reset();
				receiveDone.Reset();
			}
		}

		private void ReceivedNewMessage(string message)
		{
			var ev = ReceiveMessage;
			if(ev != null)
			{
				ev(message);
			}
		}

		public bool IsConnected
		{
			get
			{
				if (_client == null) return false;
				return _client.Connected;
			}
		}
	}
}
