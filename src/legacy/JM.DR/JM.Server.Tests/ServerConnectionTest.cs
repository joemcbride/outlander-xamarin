using System;
using System.Collections.Generic;

using NUnit.Framework;
using System.Diagnostics;

namespace JM.Server.Tests
{
    [TestFixture]
    public class ServerConnectionTest
    {
        [Test]
        public void GameListTest()
        {
            Debug.WriteLine("Starting Test\r");

            using (LoginServer s = new LoginServer())
            {
                Debug.WriteLine("Connecting...");

                if (s.Connect("account", "password"))
                {
                    Debug.WriteLine("Connection Success");
                }
                else
                {
                    Debug.WriteLine("Connection Failed");

                    return;
                }

                Debug.WriteLine("\nGame List...");

                List<Game> games = s.GetGameList();

                Game game = null;

                foreach (Game g in games)
                {
                    Debug.WriteLine(String.Format("Game: {0} {1}", g.Code, g.Name));

                    if (g.Code.Equals("DR", StringComparison.InvariantCultureIgnoreCase))
                    {
                        game = g;
                    }
                }

                if (game == null)
                {
                    game = new Game("DR", "Dragonrealms");
                }

                List<Character> chars = s.GetCharacterList(game);

                foreach (Character c in chars)
                {
                    Debug.WriteLine(String.Format("Char: {0} {1}", c.Name, c.UserID));
                }

                Debug.WriteLine("\nConnecting Character...");

                ConnectionToken token = s.ChooseCharacter(chars[0]);

                Debug.WriteLine(String.Format("Host: {0}\nPort: {1}\nKey: {2}",
                    token.GameHost, token.GamePort, token.Key));

                //GameServer gs = new GameServer();
                //gs.Connect(token, null);



                Debug.WriteLine("\nEnd Test");
            }
        }
    }
}
