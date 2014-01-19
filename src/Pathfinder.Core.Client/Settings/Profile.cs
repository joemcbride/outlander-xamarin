namespace Outlander.Core.Client
{
	public class Profile
	{
		public string Name {get;set;}
		public string Account {get;set;}
		public string Game {get;set;}
		public string Character {get;set;}

		public static Profile For(string name, string account, string game, string character)
		{
			return new Profile { Name = name, Account = account, Game = game, Character = character };
		}
	}
}