using CCGP.AspectContainer;
using System;
using System.Collections;

namespace CCGP.Server
{
    public class Phase
    {
        public readonly GameAction Owner;
        public readonly Action<IContainer> Handler;

        public Func<IContainer, GameAction, IEnumerator> Waiter;

        public Phase(GameAction owner, Action<IContainer> handler)
        {
            Owner = owner;
            Handler = handler;
        }

        // Waiter가 지정되어 있다면, 해당 코루틴을 실행하여 대기
        public IEnumerator Flow(IContainer game)
        {
            bool keyFrameHit = false;

            if(Waiter != null)
            {
                var sequence = Waiter(game, Owner);
                while(sequence.MoveNext())
                {
                    // 만약 sequence.Current가 bool이라면, true를 키프레임(조건 충족) 신호로 사용
                    bool isKeyFrame = (sequence.Current is bool b) ? b : false;
                    if(isKeyFrame)
                    {
                        keyFrameHit = true;
                        Handler(game); // 조건 충족 시점에 Handler 실행
                    }
                    yield return null; // 매 tick마다 코루틴 진행
                }
            }

            if(!keyFrameHit)
            {
                Handler(game);
            }
        }
    }
}