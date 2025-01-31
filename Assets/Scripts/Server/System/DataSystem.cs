using CCGP.AspectContainer;
using CCGP.Shared;
using System.Collections.Generic;

namespace CCGP.Server
{
    public class DataSystem : Aspect
    {
        public Match match;

        public DataSystem(uint id, List<ulong> playerIDs)
        {
            List<Player> players = new();

            for(int i = 0; i < playerIDs.Count; i++)
            {
                players.Add(new Player(playerIDs[i]));
            }

            match = new Match(id, players);
        }
    }

    public static class DataSystemExtensions
    {
        public static Match GetMatch(this IEntity game)
        {
            if(game.TryGetAspect<DataSystem>(out var dataSystem))
            {
                return dataSystem.match;
            }
            else
            {
                Logger.LogWarning<DataSystem>("Container did not have data system.");
                return null;
            }
        }
    }
}