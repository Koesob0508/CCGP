using CCGP.AspectContainer;
using CCGP.Server;
using System.Collections.Generic;

namespace CCGP.Shared
{
    public static class MainSystemFactory
    {
        public static Container Create()
        {
            Container mainSystem = new Container();

            mainSystem.AddAspect<ServerSystem>();
            mainSystem.AddAspect<NetworkSystem>();
            // ClientSystem은 ClientSystem이 자체적으로 등록

            return mainSystem;
        }
    }

    public static class GameSystemFactory
    {
        public static Container Create(uint matchID, List<ulong> players)
        {
            Container gameSystem = new Container();

            var dataSystem = new DataSystem(matchID, players);
            gameSystem.AddAspect(dataSystem);
            gameSystem.AddAspect<MatchSystem>();
            gameSystem.AddAspect<ActionSystem>();
            gameSystem.AddAspect<FlowSystem>();
            gameSystem.AddAspect<OpenMarketSystem>();
            gameSystem.AddAspect<PlayerSystem>();
            gameSystem.AddAspect<CardSystem>();

            return gameSystem;
        }
    }
}