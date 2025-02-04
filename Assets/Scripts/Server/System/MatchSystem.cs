using CCGP.AspectContainer;
using CCGP.Shared;
using System;

namespace CCGP.Server
{
    public class MatchSystem : Aspect, IObserve
    {
        public void Awake()
        {    
            this.AddObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>(), Container);
        }

        public void Sleep()
        {
            this.RemoveObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>(), Container);
        }

        private void OnPerformGameStart(object sender, object args)
        {
            var match = Container.GetMatch();
            // match의 FirstPlayerIndex를 무작위로 정한다.
            var random = new Random();
            match.FirstPlayerIndex = random.Next(0, match.Players.Count);

            Logger.Log<MatchSystem>($"게임 시작 - 첫 번째 플레이어: {match.FirstPlayerIndex}");
        }
    }
}