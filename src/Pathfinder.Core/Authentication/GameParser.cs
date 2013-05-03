using System;
using System.Collections.Generic;

namespace Pathfinder.Core.Authentication
{
    public interface IGameParser
    {
        IEnumerable<Game> Parse(string data);
    }

    public class GameParser : IGameParser
    {
        public IEnumerable<Game> Parse(string data)
        {
            var split = data.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);

            var games = new List<Game>();

            for (var i = 1; i < split.Length; i += 2)
            {
                var game = new Game
                {
                    Code = split[i].Trim(new[] {' ','\r', '\n'}),
                    Name = split[i + 1].Trim(new[] {' ', '\r', '\n'})
                };
                games.Add(game);
            }

            return games;
        }
    }
}
