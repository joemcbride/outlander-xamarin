using System.Linq;
using Pathfinder.Core;
using Pathfinder.Core.Authentication;
using console = System.Console;

namespace Pathfinder.Console
{
    class Program
    {
        static void Main(string[] args)
        {
			var gameServer = new Boostrapper().Build();

			var token = gameServer.Authenticate("DR", args[0], args[1], args[2]);
			if(token == null)
			{
				console.WriteLine("Unable to authenticate.");
				return;
			}
			gameServer.Connect(token);
			gameServer.GameState.TextLog = (msg) => {
				console.Write(msg);
			};

			while(true) {
				string stringToSend = console.ReadLine();

				gameServer.SendCommand(stringToSend + "\r\n");

				if (stringToSend == "exit") {
					break;
				}
			}
			gameServer.Disconnect();
			console.ReadLine();
        }
    }
}
