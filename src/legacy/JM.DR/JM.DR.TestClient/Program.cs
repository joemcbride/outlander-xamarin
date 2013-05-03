using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using JM.Server;

namespace JM.DR.TestClient
{
    class Program
    {
        static System.Threading.Timer timer;
        static Object lockObject = new Object();

        static void Main(String[] args)
        {
            LoginServer loginServer = new LoginServer();
            loginServer.Connect("account", "password");

            Game g = new Game("DR", "Dragonrealms");

            Character character = loginServer.GetCharacterList(g)[0];

            Console.WriteLine(String.Format("Connecting as {0}...", character.Name));

            ConnectionToken token = loginServer.ChooseCharacter(character);

            loginServer.Close();

            GameServer gameServer = new GameServer();

            Console.WriteLine(String.Format("Connecting to {0} {1}...", token.GameHost, token.GamePort));
            gameServer.Connect(token, null);

            timer = new System.Threading.Timer(new System.Threading.TimerCallback(timer_Elapsed), gameServer, new TimeSpan(0, 0, 5), new TimeSpan(0, 0, 1));

            String command = Console.ReadLine();

            while (command != null)
            {
                gameServer.Send(command);

                if (command.Equals("quit", StringComparison.InvariantCultureIgnoreCase))
                {
                    command = null;
                }
                else
                {
                    command = Console.ReadLine();
                }
            }

            gameServer.Close();
        }

        static void timer_Elapsed(Object state)
        {
            GameServer server = state as GameServer;

            server.Poll();
        }
    }
}
