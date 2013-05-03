using System;

namespace Pathfinder.Core.Authentication
{
    public interface IConnectionTokenParser
    {
        ConnectionToken Parse(string data);
    }

    public class ConnectionTokenParser : IConnectionTokenParser
    {
        private readonly char[] _charArray = {' ', '\n', '\r'};

        public ConnectionToken Parse(string data)
        {
            var result = data.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);

            var token = new ConnectionToken
            {
                GameHost = result[7].Substring(9).Trim(_charArray),
                GamePort = Int32.Parse(result[8].Substring(9).Trim(_charArray)),
                Key = result[9].Substring(4).Trim(_charArray)
            };

            return token;
        }
    }
}
