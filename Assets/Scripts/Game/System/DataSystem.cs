using CCGP.AspectContainer;
using CCGP.Shared;
using System.Collections.Generic;

namespace CCGP.Server
{
    public class DataSystem : Aspect, IActivatable
    {
        Game Game => Container as Game;
        public Match match;

        public void Activate()
        {
            var board = BoardFactory.Create();

            List<Player> players = new();

            for (int i = 0; i < Game.PlayerInfos.Count; i++)
            {
                List<Card> initialDeck = CardFactory.CreateDeck("InitialDeck", i);

                var player = new Player(i, Game.PlayerInfos[i]);
                player[Zone.Deck].AddRange(initialDeck);

                players.Add(player);
            }

            match = new Match(board, players);
        }

        public void Deactivate()
        {
            throw new System.NotImplementedException();
        }
    }

    public static class DataSystemExtensions
    {
        public static Match GetMatch(this IContainer game)
        {
            if(game.TryGetAspect<DataSystem>(out var dataSystem))
            {
                return dataSystem.match;
            }
            else
            {
                LogUtility.LogWarning<DataSystem>("Container did not have data system.");
                return null;
            }
        }
    }
}