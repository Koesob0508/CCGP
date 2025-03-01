using CCGP.AspectContainer;
using CCGP.Shared;

namespace CCGP.Server
{
    public class CostSystem : Aspect, IActivatable
    {
        public void Activate()
        {
            this.AddObserver(OnValidateCardPlayAction, Global.ValidateNotification<PlayCardAction>());
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnValidateCardPlayAction, Global.ValidateNotification<PlayCardAction>());
        }

        private void OnValidateCardPlayAction(object sender, object args)
        {
            var action = sender as PlayCardAction;
            var validator = args as Validator;

            // 앞선 시스템(== Target System)에서 이미 불합격이라면 검증 안해도 됨
            if (!validator.IsValid) return;

            if (action.Card.TryGetAspect(out Target target))
            {
                // Target 안에는 Tile이 있습니다.
                var selectedTile = target.Selected;
                // Tile은 Container이고, Cost를 갖고 있을 수 있습니다.
                // 만일 Cost를 갖고 있지 않다면 return
                if (!selectedTile.TryGetAspect(out Cost cost))
                {
                    LogUtility.Log<CostSystem>("Cost 요구하지 않음");
                    return;
                }

                var targetPlayer = action.Player;
                // 만일 Cost를 갖고 있다면, Cost로부터
                // Player의 Resource을 확인합니다.
                // 만일 Player가 Resource가 부족하다면 Invalidate
                switch (cost.Type)
                {
                    case ResourceType.Lunar:
                        if (targetPlayer.Lunar < cost.Amount)
                        {
                            LogUtility.LogWarning<CostSystem>("Lunar Cost 만족하지 않음", colorName: ColorCodes.Logic);
                            validator.Invalidate();
                        }
                        break;
                    case ResourceType.Marsion:
                        if (targetPlayer.Marsion < cost.Amount)
                        {
                            LogUtility.LogWarning<CostSystem>("Marsion Cost 만족하지 않음", colorName: ColorCodes.Logic);
                            validator.Invalidate();
                        }
                        break;
                    case ResourceType.Water:
                        if (targetPlayer.Water < cost.Amount)
                        {
                            LogUtility.LogWarning<CostSystem>("Water Cost 만족하지 않음", colorName: ColorCodes.Logic);
                            validator.Invalidate();
                        }
                        break;
                }
            }
            else
            {
                LogUtility.Log<ConditionSystem>("Target 없음");
            }
        }
    }
}