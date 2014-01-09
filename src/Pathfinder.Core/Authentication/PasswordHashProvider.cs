using System.Text;

namespace Outlander.Core.Authentication
{
    public interface IPasswordHashProvider
    {
        string Hash(string token, string password);
    }

    public class PasswordHashProvider : IPasswordHashProvider
    {
        public string Hash(string token, string password)
        {
			byte[] encrypted = new byte[33];
			for (int i = 0; i < 32 && password.Length > i && token.Length > i; i++)
			{
				encrypted[i] = (byte) ((token[i]  ^ (password[i] - 32)) + 32);
			}

			return Encoding.UTF8.GetString(encrypted);
        }
    }
}
