using System;

namespace Pathfinder.Core.Authentication
{
    public sealed class ConnectionToken
    {
        public ConnectionToken()
        {
            GameHost = String.Empty;
            GamePort = -1;
            Key = String.Empty;
        }

        public string GameHost { get; set; }
        public int GamePort { get; set; }
        public string Key { get; set; }

        public Boolean IsValid
        {
            get
            {
                if (!String.IsNullOrEmpty(GameHost) & (GamePort > 0) & !String.IsNullOrEmpty(Key))
                {
                    return true;
                }

                return false;
            }
        }
    }
}
