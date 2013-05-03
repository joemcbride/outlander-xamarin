using System.Linq;
using Pathfinder.Core;
using Pathfinder.Core.Authentication;

namespace Pathfinder.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var account = args[0];
            var password = args[1];

            var authServer = new AuthenticationServer("eaccess.play.net", 7900);
            var authenticated = authServer.Authenticate(account, password);

            if (!authenticated)
            {
                System.Console.WriteLine("Authentication failed!");
                System.Console.ReadKey();
                return;
            }

            var gameList = authServer.GetGameList();
            gameList
                .Select(g => g.Code + ", " + g.Name)
                .Apply(System.Console.WriteLine);

            var characters = authServer.GetCharacterList("DR").ToList();
            characters
                .Select(c => c.CharacterId + ", " + c.Name)
                .Apply(System.Console.WriteLine);

            var token = authServer.ChooseCharacter(characters[0].CharacterId);

            authServer.Close();
        }
    }

}
