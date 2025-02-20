using CCGP.AspectContainer;
using CCGP.Shared;

namespace CCGP.Server
{
    public class ConditionSystem : Aspect, IActivatable
    {
        public void Activate()
        {
            this.AddObserver(OnValidateCardPlayAction, Global.ValidateNotification<CardPlayAction>());
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnValidateCardPlayAction, Global.ValidateNotification<CardPlayAction>());
        }

        private void OnValidateCardPlayAction(object sender, object args)
        {
            var action = sender as CardPlayAction;
            var validator = args as Validator;

            // 앞선 시스템(== Target System)에서 이미 불합격이라면 검증 안해도 됨
            if (!validator.IsValid) return;

            if (action.Card.TryGetAspect(out Target target))
            {
                // Target 안에는 Tile이 있습니다.
                var selectedTile = target.Selected;
                // Tile은 Container이고, Condition을 갖고 있을 수 있습니다.
                // 만일 Condition을 갖고 있지 않다면 return
                if (!selectedTile.TryGetAspect(out Condition condition))
                {
                    LogUtility.Log<ConditionSystem>("Condition 요구하지 않음");
                    return;
                }

                var targetPlayer = action.Player;
                // 만일 Condition을 갖고 있다면, Condition으로부터
                // Player의 조건을 확인합니다.
                // 만일 Player가 조건을 만족하지 않았다면 Invalidate
                switch (condition.Type)
                {
                    case ConditionType.Emperor:
                        if (targetPlayer.EmperorInfluence < condition.Amount)
                        {
                            LogUtility.LogWarning<ConditionSystem>("Emperor Influence 만족하지 않음", colorName: ColorCodes.Logic);
                            validator.Invalidate();
                        }
                        break;
                    case ConditionType.SpacingGuild:
                        if (targetPlayer.SpacingGuildInfluence < condition.Amount)
                        {
                            LogUtility.LogWarning<ConditionSystem>("Spacing Guild Influence 만족하지 않음", colorName: ColorCodes.Logic);
                            validator.Invalidate();
                        }
                        break;
                    case ConditionType.BeneGesserit:
                        if (targetPlayer.BeneGesseritInfluence < condition.Amount)
                        {
                            LogUtility.LogWarning<CostSystem>("Bene Gesserit Influence 만족하지 않음", colorName: ColorCodes.Logic);
                            validator.Invalidate();
                        }
                        break;
                    case ConditionType.Fremen:
                        if (targetPlayer.FremenInfluence < condition.Amount)
                        {
                            LogUtility.LogWarning<CostSystem>("Fremen Influence 만족하지 않음", colorName: ColorCodes.Logic);
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