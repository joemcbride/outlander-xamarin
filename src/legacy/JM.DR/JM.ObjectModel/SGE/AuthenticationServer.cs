using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

using JM.DR.ObjectModel;
using System.Text.RegularExpressions;

namespace JM.DR.ObjectModel.SGE
{
    public sealed class AuthenticationServer : IDisposable
    {
        #region Private Members
        // remote endpoint for the socket
        private IPEndPoint m_RemoteEndPoint;
        private string _host;
        private int _port;

        // TCP/IP socket
        private SimpleSocket m_Socket;
        #endregion

        #region Constructors
        public AuthenticationServer(String host, Int32 port)
        {
            _host = host;
            _port = port;

            // Establish the remote endpoint for the socket.
            m_RemoteEndPoint = new IPEndPoint(Dns.GetHostEntry(host).AddressList[0], port);

            // Create a TCP/IP  socket.
            m_Socket = new SimpleSocket();
        }
        #endregion

        private void Connect()
        {
            m_Socket.Connect(_host, _port);
        }

        #region Public Methods
        public Boolean Authenticate(String account, String password)
        {
            if (String.IsNullOrEmpty(account) || String.IsNullOrEmpty(password))
            {
                return false;
            }

            try
            {
                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    if (!this.m_Socket.Connected == true)
                    {
                        this.Connect();
                    }

                    String passwordHash = m_Socket.SendAndReceive("K");

                    Debug.WriteLine(String.Format("Echoed passwordHash = {0}", passwordHash));

                    Byte[] pHash = Encoding.UTF8.GetBytes(passwordHash);

                    Byte[] pass = Encoding.UTF8.GetBytes(password);

                    for (Int32 i = 0; i < pass.Length; i++)
                    {
                        pass[i] = (Byte)(((pass[i] - 0x20) ^ pHash[i]) + 0x20);
                    }

                    String hashedPassword = Encoding.UTF8.GetString(pass);

                    String message = String.Format("A\t{0}\t{1}", account, hashedPassword);

                    String result = m_Socket.SendAndReceive(message);

                    Debug.WriteLine(String.Format("Rec: {0}", result));

                    if (result.Contains("KEY"))
                    {
                        return true;
                    }

                }
                catch (ArgumentNullException ane)
                {
                    Debug.WriteLine(String.Format("ArgumentNullException : {0}", ane.ToString()));
                }
                catch (SocketException se)
                {
                    Debug.WriteLine(String.Format("SocketException : {0}", se.ToString()));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(String.Format("Unexpected exception : {0}", e.ToString()));
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return false;
        }

        public List<Game> GetGameList()
        {
            String result = m_Socket.SendAndReceive("M");

            String[] split = result.Split(new String[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);

            List<Game> games = new List<Game>();

            Game game = null;

            for (Int32 i = 1; i < split.Length; i++)
            {
                if ((i % 2) == 0)
                {
                    game.Name = split[i].Trim(new Char[] { '\r', '\n' });
                }
                else
                {
                    game = new Game();
                    game.Code = split[i].Trim(new Char[] { '\r', '\n' });
                    games.Add(game);
                }
            }

            return games;
        }

        public List<Character> GetCharacterList(Game game)
        {
            String response;

            response = m_Socket.SendAndReceive(String.Format("F\t{0}", game.Code));
            //Debug.WriteLine("F: " + response);

            response = m_Socket.SendAndReceive(String.Format("G\t{0}", game.Code));
            //Debug.WriteLine("G: " + response);

            //response = SendCommand(String.Format("P\t{0}", game.Code));
            //Debug.WriteLine("P: " + response);

            response = m_Socket.SendAndReceive("C");
            //Debug.WriteLine("C: " + response);

            String[] characterData = response.Split(new String[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);

            List<String> list = new List<String>();
            list.AddRange(characterData);

            for (Int32 i = 0; i < 5; i++)
            {
                list.RemoveAt(0);
            }

            List<Character> characters = new List<Character>();

            for (Int32 i = 0; i < list.Count; i += 2)
            {
                Character character = new Character(
                    list[i + 1].Trim(new Char[] { '\r', '\n' }),
                    list[i].Trim(new Char[] { '\r', '\n' })
                );

                characters.Add(character);
            }

            return characters;
        }

        public ConnectionToken ChooseCharacter(Character character)
        {
            String connectString = String.Format("L\t{0}\tPLAY\r\n", character.UserID);

            String[] result = m_Socket.SendAndReceive(connectString).Split(new String[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);

            ConnectionToken token = new ConnectionToken();

            foreach (String s in result)
            {
                if (Regex.IsMatch(s, "GAMEHOST=.*"))
                {
                    token.GameHost = s.Substring(9).Trim(new Char[] { '\n', '\r' });
                }
                else if (Regex.IsMatch(s, "GAMEPORT=.*"))
                {
                    token.GamePort = Int32.Parse(s.Substring(9).Trim(new Char[] { '\n', '\r' }));
                }
                else if (Regex.IsMatch(s, "KEY=.*"))
                {
                    token.Key = s.Substring(4).Trim(new Char[] { '\n', '\r' });
                }
            }

            return token;
        }

        public void Close()
        {
            if (m_Socket == null)
                return;

            try
            {
                m_Socket.Close();
            }
            catch (ArgumentNullException ane)
            {
                Debug.WriteLine(String.Format("ArgumentNullException : {0}", ane.ToString()));
            }
            catch (SocketException se)
            {
                Debug.WriteLine(String.Format("SocketException : {0}", se.ToString()));
            }
            catch (Exception e)
            {
                Debug.WriteLine(String.Format("Unexpected exception : {0}", e.ToString()));
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                m_Socket.Shutdown(SocketShutdown.Both);

                this.Close();
            }
            catch (ArgumentNullException ane)
            {
                Debug.WriteLine(String.Format("ArgumentNullException : {0}", ane.ToString()));
            }
            catch (SocketException se)
            {
                Debug.WriteLine(String.Format("SocketException : {0}", se.ToString()));
            }
            catch (Exception e)
            {
                Debug.WriteLine(String.Format("Unexpected exception : {0}", e.ToString()));
            }
        }

        #endregion
    }
}
