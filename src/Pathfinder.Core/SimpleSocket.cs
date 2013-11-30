namespace Pathfinder.Core
{
    using System;
    using System.Net.Sockets;
    using System.Text;

	public interface ISimpleSocket : IDisposable
    {
		Exception LastError { get; }

        void Close();
        void Connect(string host, int port);
        bool Send(string data);
        string SendAndReceive(string data);
        string Receive();
        void Shutdown(SocketShutdown option);
        bool Connected { get; }
    }

    public class SimpleSocket : Socket, ISimpleSocket
    {
        private readonly byte[] _bytes;
        private const string ReturnLineFeed = "\r\n";

        public SimpleSocket()
            : this(1024)
        { }

        public SimpleSocket(int bufferSize)
            : this(bufferSize, AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        { }

        public SimpleSocket(int bufferSize, AddressFamily addressFamily,
            SocketType socketType, ProtocolType protocolType)
            : base(addressFamily, socketType, protocolType)
        {
            _bytes = new byte[bufferSize];
            IncludeCarriageReturnLineFeed = true;
        }

        public bool IncludeCarriageReturnLineFeed { get; set; }
        public Exception LastError { get; set; }

        public bool Send(string data)
        {
            byte[] message = IncludeCarriageReturnLineFeed
                ? Encoding.UTF8.GetBytes(data + ReturnLineFeed)
                : Encoding.UTF8.GetBytes(data);

            try
            {
                Send(message);
                return true;
            }
            catch (Exception exc)
            {
                LastError = exc;
                return false;
            }
        }

        public String Receive()
        {
            try
            {
                int bytesRec = Receive(_bytes);

                return Encoding.UTF8.GetString(_bytes, 0, bytesRec);
            }
            catch (Exception exc)
            {
                LastError = exc;
            }

            return String.Empty;
        }

        public String SendAndReceive(String data)
        {
            Send(data);

            return Receive();
        }

        public void ClearError()
        {
            LastError = null;
        }
    }
}
