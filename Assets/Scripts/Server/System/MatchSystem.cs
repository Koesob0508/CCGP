using CCGP.AspectContainer;
using CCGP.Shared;
using System;

namespace CCGP.Server
{
    public class MatchSystem : Aspect, IObserve
    {
        public void Awake()
        {    
            this.AddObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>(), Entity);
        }

        public void Sleep()
        {
            this.RemoveObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>(), Entity);
        }

        private void OnPerformGameStart(object sender, object args)
        {
            var match = Entity.GetMatch();
            // match의 FirstPlayerIndex를 무작위로 정한다.
            var random = new Random();
            match.FirstPlayerIndex = random.Next(0, match.Players.Count);
            // 정해진 FirstPlayerIndex에 따라서 match의 CurrentPlayerIndex도 정한다.
            match.CurrentPlayerIndex = match.FirstPlayerIndex;

            Logger.Log<MatchSystem>($"게임 시작 - 첫 번째 플레이어: {match.FirstPlayerIndex}");
        }
    }
}