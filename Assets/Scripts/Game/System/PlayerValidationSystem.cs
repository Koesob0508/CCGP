using CCGP.AspectContainer;
using CCGP.Shared;

namespace CCGP.Server
{
    public class PlayerValidationSystem : Aspect, IActivatable
    {
        public void Activate()
        {
            this.AddObserver(OnValidateTryPlayCard, Global.ValidateNotification<TryPlayCardAction>());
            this.AddObserver(OnValidateTryOpenCards, Global.ValidateNotification<TryOpenCardsAction>());
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnValidateTryPlayCard, Global.ValidateNotification<TryPlayCardAction>());
            this.RemoveObserver(OnValidateTryOpenCards, Global.ValidateNotification<TryOpenCardsAction>());
        }

        private void OnValidateTryPlayCard(object sender, object args)
        {
            LogUtility.Log<PlayerValidationSystem>("On Validate TryPlayCard", colorName: ColorCodes.Logic);
            var action = sender as TryPlayCardAction;
            var validator = args as Validator;

            // 앞선 시스템에서 이미 불합격이라면 검증 안해도 됨
            if (!validator.IsValid) return;

            if (action.Card == null)
            {
                LogUtility.LogWarning<PlayerSystem>("Card is null", colorName: ColorCodes.Logic);
                validator.Invalidate();
                return;
            }

            if (action.Card.OwnerIndex != Container.GetMatch().CurrentPlayerIndex)
            {
                LogUtility.LogWarning<PlayerSystem>("Not your turn", colorName: ColorCodes.Logic);
                validator.Invalidate();
                return;
            }

            if (action.Card.Zone != Zone.Hand)
            {
                LogUtility.LogWarning<PlayerSystem>("Card is not in Hand", colorName: ColorCodes.Logic);
                validator.Invalidate();
                return;
            }

            if (action.Card.Space == Space.None)
            {
                LogUtility.LogWarning<PlayerSystem>("Card has no space", colorName: ColorCodes.Logic);
                validator.Invalidate();
                return;
            }

            if (Container.GetMatch().Players[action.Card.OwnerIndex].TurnActionCount == 0)
            {
                LogUtility.LogWarning<PlayerSystem>("Turn action count is 0", colorName: ColorCodes.Logic);
                validator.Invalidate();
                return;
            }

            if (Container.GetMatch().Players[action.Card.OwnerIndex].UsedAgentCount == Container.GetMatch().Players[action.Card.OwnerIndex].TotalAgentCount)
            {
                LogUtility.LogWarning<PlayerSystem>("Agent action count is 0", colorName: ColorCodes.Logic);
                validator.Invalidate();
                return;
            }
        }

        private void OnValidateTryOpenCards(object sender, object args)
        {

        }
    }
}