using System.Linq;
using Outlander.Core;
using Outlander.Core.Authentication;
using console = System.Console;

namespace Outlander.Console
{
    class Program
    {
        static void Main(string[] args)
        {
			var gameServer = new Bootstrapper().Build();

			var token = gameServer.Authenticate("DR", args[0], args[1], args[2]);
			if(token == null)
			{
				console.WriteLine("Unable to authenticate.");
				return;
			}
			gameServer.Connect(token);
			gameServer.GameState.TextLog += (msg) => {
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
