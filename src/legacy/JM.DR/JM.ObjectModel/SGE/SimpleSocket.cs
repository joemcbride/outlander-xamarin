using System;
using System.Net.Sockets;
using System.Text;

namespace JM.DR.ObjectModel.SGE
{
    public class SimpleSocket : Socket
    {
        #region Private Members
        private Byte[] m_Bytes;
        private Boolean m_IncludeCarriageReturnLineFeed;
        private Exception m_LastError;

        private static String ReturnLineFeed = "\r\n";
        #endregion

        #region Constructors
        public SimpleSocket()
            : this(1024)
        { }

        public SimpleSocket(Int32 bufferSize)
            : this(bufferSize, AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        { }

        public SimpleSocket(Int32 bufferSize, AddressFamily addressFamily,
            SocketType socketType, ProtocolType protocolType)
            : base(addressFamily, socketType, protocolType)
        {
            m_Bytes = new Byte[bufferSize];
            m_IncludeCarriageReturnLineFeed = true;
        }
        #endregion

        #region Public Properties
        public Boolean IncludeCarriageReturnLineFeed
        {
            get
            {
                return m_IncludeCarriageReturnLineFeed;
            }
            set
            {
                m_IncludeCarriageReturnLineFeed = value;
            }
        }

        public Exception LastError
        {
            get
            {
                return m_LastError;
            }
            set
            {
                m_LastError = value;
            }
        }
        #endregion

        #region Public Methods
        public Boolean Send(String data)
        {
            Byte[] message;

            if (IncludeCarriageReturnLineFeed == true)
            {
                message = Encoding.UTF8.GetBytes(data + ReturnLineFeed);
            }
            else
            {
                message = Encoding.UTF8.GetBytes(data);
            }

            try
            {
                this.Send(message);

                return true;
            }
            catch (Exception exc)
            {
                this.LastError = exc;

                return false;
            }
        }

        public String Receive()
        {
            try
            {
                Int32 bytesRec = this.Receive(m_Bytes);

                return Encoding.UTF8.GetString(m_Bytes, 0, bytesRec);
            }
            catch (Exception exc)
            {
                this.LastError = exc;
            }

            return String.Empty;
        }

        public String SendAndReceive(String data)
        {
            this.Send(data);

            return this.Receive();
        }

        public void ClearError()
        {
            LastError = null;
        }
        #endregion
    }
}
