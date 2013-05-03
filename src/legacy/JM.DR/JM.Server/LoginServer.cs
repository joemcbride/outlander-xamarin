using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace JM.Server
{
    public class LoginServer : IDisposable
    {
        public static String SGE_HOST = "eaccess.play.net";
        public static Int32 SGE_PORT = 7900;

        // data buffer for incoming data
        private Byte[] bytes = new Byte[1024];

        // remote endpoing for the socket
        private IPEndPoint m_RemoteEndPoint;

        // TCP/IP socket
        private Socket m_Socket;

        public Boolean Connect(String account, String password)
        {
            try
            {
                // Establish the remote endpoint for the socket.
                m_RemoteEndPoint = new IPEndPoint(Dns.GetHostEntry(SGE_HOST).AddressList[0], SGE_PORT);

                // Create a TCP/IP  socket.
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    m_Socket.Connect(m_RemoteEndPoint);

                    String passwordHash = SendCommand("K");

                    Debug.WriteLine(String.Format("Echoed passwordHash = {0}", passwordHash));

                    Byte[] pHash = Encoding.UTF8.GetBytes(passwordHash);

                    Byte[] pass = Encoding.UTF8.GetBytes(password);

                    for (Int32 i = 0; i < pass.Length; i++)
                    {
                        pass[i] = (Byte)(((pass[i] - 0x20) ^ pHash[i]) + 0x20);
                    }

                    String hashedPassword = Encoding.UTF8.GetString(pass);

                    String message = String.Format("A\t{0}\t{1}", account, hashedPassword);

                    String result = SendCommand(message);

                    Debug.WriteLine(String.Format("Rec: {0}", result));

                    if (!result.Contains("KEY"))
                    {
                        return false;
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

            return true;

        }

        public List<Game> GetGameList()
        {
            String result = SendCommand("M");

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

            response = SendCommand(String.Format("F\t{0}", game.Code));
            //Debug.WriteLine("F: " + response);

            response = SendCommand(String.Format("G\t{0}", game.Code));
            //Debug.WriteLine("G: " + response);

            //response = SendCommand(String.Format("P\t{0}", game.Code));
            //Debug.WriteLine("P: " + response);

            response = SendCommand("C");
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

            String[] result = SendCommand(connectString).Split(new String[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);

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
                m_Socket.Shutdown(SocketShutdown.Both);
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

        private String SendCommand(String command)
        {
            Byte[] message = Encoding.UTF8.GetBytes(command + "\r\n");

            m_Socket.Send(message);

            Int32 bytesRec = m_Socket.Receive(bytes);

            return Encoding.UTF8.GetString(bytes, 0, bytesRec);
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Close();
        }

        #endregion
    }
}
