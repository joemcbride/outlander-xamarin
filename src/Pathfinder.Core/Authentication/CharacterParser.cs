using System;
using System.Collections.Generic;

namespace Pathfinder.Core.Authentication
{
    public interface ICharacterParser
    {
        IEnumerable<Character> Parse(string data);
    }

    public class CharacterParser : ICharacterParser
    {
        public IEnumerable<Character> Parse(string data)
        {
            var characterData = data.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);

            var characters = new List<Character>();

            for (var i = 5; i < characterData.Length; i += 2)
            {
                var character = new Character
                {
                    CharacterId = characterData[i].Trim('\r', '\n'),
                    Name = characterData[i + 1].Trim('\r', '\n')
                };

                characters.Add(character);
            }

            return characters;
        }
    }
}
