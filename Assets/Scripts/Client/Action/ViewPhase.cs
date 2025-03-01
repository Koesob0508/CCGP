using CCGP.AspectContainer;
using System;
using System.Collections;

namespace CCGP.Client
{
    public class ViewPhase
    {
        public readonly ViewAction Owner;
        public readonly Action<IContainer> Handler;

        public Func<IContainer, ViewAction, IEnumerator> Awaiter;

        public ViewPhase(ViewAction owner, Action<IContainer> handler)
        {
            Owner = owner;
            Handler = handler;
        }

        public IEnumerator Flow(IContainer game)
        {
            bool keyFrameHit = false;

            if (Awaiter != null)
            {
                var awaitSequence = Awaiter(game, Owner);
                while (awaitSequence.MoveNext())
                {
                    // 만약 sequence.Current가 bool이라면, true를 키프레임(조건 충족) 신호로 사용
                    bool isKeyFrame = (awaitSequence.Current is bool b) ? b : false;
                    if (isKeyFrame)
                    {
                        keyFrameHit = true;
                        Handler(game); // 조건 충족 시점에 Handler 실행
                    }
                    yield return null; // 매 tick마다 코루틴 진행
                }
            }

            if (!keyFrameHit)
            {
                Handler(game);
            }
        }
    }
}