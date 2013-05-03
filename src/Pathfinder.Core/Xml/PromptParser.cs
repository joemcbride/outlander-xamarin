using System;
using System.Xml.Linq;
using Pathfinder.Core.Events;

namespace Pathfinder.Core.Xml
{
    public class PromptParser
    {
        private static readonly ILog Log = LogManager.GetLog(typeof (PromptParser));

        public bool Matches(string data)
        {
            return !string.IsNullOrWhiteSpace(data) && data.StartsWith("<prompt");
        }

        public string Parse(string data)
        {
            string prompt = data;

            try
            {
                var element = XElement.Parse(data.Trim());
                var time = Double.Parse(element.Attribute("time").Value).UnixTimeStampToDateTime();
                prompt = element.Value;
            }
            catch (Exception exc)
            {
                Log.Error(exc);
            }

            return prompt;
        }
    }
}
