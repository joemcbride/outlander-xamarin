using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;

namespace JM.Server
{
    public class GameServer
    {
        private IPEndPoint m_RemoteEndPoint;
        private Socket m_Socket;
        private Byte[] bytes = new Byte[1024];

        private String m_StrormFrontVersion = "1.0.1.22";
        private String m_WizardVersion = "2.04";

        public void Connect(ConnectionToken token, AsyncCallback receiveCallback)
        {
            Debug.WriteLine("GameServer - Connect Called.");

            // Establish the remote endpoint for the socket.
            m_RemoteEndPoint = new IPEndPoint(Dns.GetHostEntry(token.GameHost).AddressList[0], token.GamePort);

            // Create a TCP/IP  socket.
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            m_Socket.Connect(m_RemoteEndPoint);

            //m_Socket.BeginReceive()

            //String connectionString = String.Format("{0}\r\n/FE:WIZARD /VERSION:{1} /P:WIN_XP \r\n", token.Key, m_StrormFrontVersion);

            String connectionString = String.Format("{0}\r\n/FE:JAVA", token.Key);

            //Send(token.Key + "\r\n");
            //Console.WriteLine(Receive());

            Send(connectionString);
            //Console.WriteLine(Receive());

            //m_Socket.Blocking = false;

            //Send(connectionString);

            //Console.WriteLine(Receive());
        }

        public Boolean Poll()
        {
            //StateObject so = new StateObject();
            //so.workSocket = m_Socket;

            //m_Socket.BeginReceive(so.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(Read_Callback), so);

            if (m_Socket == null || m_Socket.Connected == false)
            {
                return false;
            }

            Int32 bytesRec = m_Socket.Receive(bytes);

            if (bytesRec > 0)
            {
                String s = Encoding.UTF8.GetString(bytes, 0, bytesRec);

                Console.Write(s);

                return true;
            }

            return false;
        }

        private void Read_Callback(IAsyncResult result)
        {
            StateObject so = (StateObject)result.AsyncState;
            Socket s = so.workSocket;

            Int32 read = s.EndReceive(result);

            Debug.WriteLine(String.Format("conn: {0} read: {1}", s.Connected, read));

            if (read > 0)
            {
                so.sb.Append(Encoding.ASCII.GetString(so.buffer, 0, read));
                s.BeginReceive(so.buffer, 0, StateObject.BufferSize, 0,
                                         new AsyncCallback(Read_Callback), so);
            }
            else
            {
                if (so.sb.Length > 1)
                {
                    //All of the data has been read, so displays it to the console
                    string strContent;
                    strContent = so.sb.ToString();
                    Console.WriteLine(String.Format("Read {0} byte from socket" +
                                       "data = {1} ", strContent.Length, strContent));
                }
                //s.Close();
            }
        }

        public void Send(String command)
        {
            Byte[] message = Encoding.UTF8.GetBytes(command + "\r\n");

            //m_Socket.Blocking = true;

            m_Socket.Send(message);

            //m_Socket.Blocking = false;
        }

        public String Receive()
        {
            //if (m_Socket.Connected == false)
            //    return String.Empty;

            //EndPoint ep = (EndPoint)m_RemoteEndPoint;

            //Int32 bytesRec = m_Socket.ReceiveFrom(bytes, ref ep);

            Int32 bytesRec = m_Socket.Receive(bytes);

            return Encoding.UTF8.GetString(bytes, 0, bytesRec);
        }

        public void Close()
        {
            m_Socket.Close();
        }
    }

    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
}
