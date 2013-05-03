using System.Text;

namespace Pathfinder.Core.Authentication
{
    public interface IPasswordHashProvider
    {
        string Hash(string token, string password);
    }

    public class PasswordHashProvider : IPasswordHashProvider
    {
        public string Hash(string token, string password)
        {
            byte[] tokenBytes = Encoding.UTF8.GetBytes(token);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            for (var i = 0; i < passwordBytes.Length; i++)
            {
                passwordBytes[i] = (byte) (((passwordBytes[i] - 0x20) ^ tokenBytes[i]) + 0x20);
            }

            return Encoding.UTF8.GetString(passwordBytes);
        }
    }
}
