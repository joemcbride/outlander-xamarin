using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;

using JM.DR.ObjectModel.SGE;

namespace JM.DR.ObjectModel
{
    public class OldGameServer
    {
        private IPEndPoint m_RemoteEndPoint;
        private Socket m_Socket;
        private Byte[] bytes = new Byte[3072];

        private String m_StormFrontVersion = "1.0.1.22";
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

            //String connectionString = String.Format("{0}\r\n/FE:WIZARD /VERSION:{1} /P:{2} /XML\r\n", token.Key, m_StormFrontVersion, Environment.OSVersion.Platform);

            String connectionString = String.Format("{0}\r\n/FE:JAVA", token.Key);

            //Send(token.Key + "\r\n");
            //Console.WriteLine(Receive());

            Send(connectionString);
            //Console.WriteLine(Receive());

            //m_Socket.Blocking = false;

            //Send(connectionString);

            //Console.WriteLine(Receive());
        }

        public String Poll()
        {
            if (m_Socket == null || m_Socket.Connected == false)
            {
                return String.Empty;
            }

            Int32 bytesRec = m_Socket.Receive(bytes);

            if (bytesRec > 0)
            {
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }

            return String.Empty;
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
