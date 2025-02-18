using CCGP.AspectContainer;
using CCGP.Shared;
using UnityEngine;

namespace CCGP.Client
{
    public static class ClientFactory
    {
        public static Client Create()
        {
            var client = new Client();

            var gameView = Object.FindFirstObjectByType<GameView>();
            var matchView = Object.FindFirstObjectByType<MatchView>();
            var boardView = Object.FindFirstObjectByType<BoardView>();
            var playerView = Object.FindFirstObjectByType<PlayerView>();
            var turnView = Object.FindFirstObjectByType<TurnView>();

            client.AddAspect(gameView);
            client.AddAspect(matchView);
            client.AddAspect(boardView);
            client.AddAspect(playerView);
            client.AddAspect(turnView);
            client.AddAspect<ClientMessageSystem>();

            return client;
        }
    }
}