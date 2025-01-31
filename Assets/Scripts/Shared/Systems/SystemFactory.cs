using CCGP.AspectContainer;
using CCGP.Server;
using System.Collections.Generic;

namespace CCGP.Shared
{
    public static class MainSystemFactory
    {
        public static Entity Create()
        {
            Entity mainSystem = new Entity();

            mainSystem.AddAspect<ServerSystem>();
            mainSystem.AddAspect<NetworkSystem>();
            // ClientSystem은 ClientSystem이 자체적으로 등록

            return mainSystem;
        }
    }

    public static class GameSystemFactory
    {
        public static Entity Create(uint matchID, List<ulong> players)
        {
            Entity gameSystem = new Entity();

            var dataSystem = new DataSystem(matchID, players);
            gameSystem.AddAspect(dataSystem);
            gameSystem.AddAspect<MatchSystem>();
            gameSystem.AddAspect<ActionSystem>();
            gameSystem.AddAspect<FlowSystem>();

            return gameSystem;
        }
    }
}