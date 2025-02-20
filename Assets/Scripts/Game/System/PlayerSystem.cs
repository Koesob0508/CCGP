using CCGP.AspectContainer;
using CCGP.Shared;
using System.Collections.Generic;
using UnityEngine;

namespace CCGP.Server
{
    public class PlayerSystem : Aspect, IActivatable
    {
        public void Activate()
        {
            // Phase notify
            this.AddObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>(), Container);
            this.AddObserver(OnPerformRoundStart, Global.PerformNotification<RoundStartAction>(), Container);
            this.AddObserver(OnPerformTurnStart, Global.PerformNotification<TurnStartAction>(), Container);

            // Client message notify
            this.AddObserver(OnReceivedTryPlayCard, Global.MessageNotification(GameCommand.TryPlayCard), Container);

            // Validate notify
            this.AddObserver(OnValidateTryCardPlay, Global.ValidateNotification<TryPlayCardAction>());
            this.AddObserver(OnValidateCardPlay, Global.ValidateNotification<CardPlayAction>());

            this.AddObserver(OnPerformCardsDraw, Global.PerformNotification<CardsDrawAction>(), Container);
            this.AddObserver(OnPerformTryCardPlay, Global.PerformNotification<TryPlayCardAction>(), Container);
            this.AddObserver(OnPerformCardPlay, Global.PerformNotification<CardPlayAction>(), Container);

        }

        public void Deactivate()
        {
            this.RemoveObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>(), Container);
            this.RemoveObserver(OnPerformRoundStart, Global.PerformNotification<RoundStartAction>(), Container);
            this.RemoveObserver(OnPerformTurnStart, Global.PerformNotification<TurnStartAction>(), Container);

            this.RemoveObserver(OnReceivedTryPlayCard, Global.MessageNotification(GameCommand.TryPlayCard), Container);

            this.RemoveObserver(OnValidateTryCardPlay, Global.ValidateNotification<TryPlayCardAction>());
            this.RemoveObserver(OnValidateCardPlay, Global.ValidateNotification<CardPlayAction>());

            this.RemoveObserver(OnPerformCardsDraw, Global.PerformNotification<CardsDrawAction>(), Container);
            this.RemoveObserver(OnPerformTryCardPlay, Global.PerformNotification<TryPlayCardAction>(), Container);
            this.RemoveObserver(OnPerformCardPlay, Global.PerformNotification<CardPlayAction>(), Container);
        }

        private void OnPerformGameStart(object sender, object args)
        {
            foreach (var player in Container.GetMatch().Players)
            {
                ShuffleDeck(player);
            }
        }

        private void OnPerformRoundStart(object sender, object args)
        {
            foreach (var player in Container.GetMatch().Players)
            {
                // TurnCount랑 AgentCount 회복 필요
                var action = new CardsDrawAction(player, Player.InitialHand);
                Container.AddReaction(action);
            }
        }

        private void OnPerformTurnStart(object sender, object args)
        {
            var action = args as TurnStartAction;
            RestoreTurnActionCount(Container.GetMatch().Players[action.TargetPlayerIndex]);
        }

        private void OnPerformCardsDraw(object sender, object args)
        {
            var action = args as CardsDrawAction;

            action.Cards = Draw(action.Player, action.Amount);
        }

        #region Try Play Card Action

        private void OnReceivedTryPlayCard(object sender, object args)
        {
            LogUtility.Log<PlayerSystem>("Received OnTryCardPlay", colorName: ColorCodes.Logic);
            var sData = args as SerializedData;
            var sCard = sData.Get<SerializedCard>();

            // sCard 정보를 이용해서 Player로부터 해당 Card를 직접 찾아내야함
            var match = Container.GetMatch();
            var player = match.Players[sCard.OwnerIndex];

            player[sCard.Zone].TryGetCard(sCard.GUID, out var card);

            // CardPlayTry로 대체
            // var action = new CardPlayAction(player, card);
            var action = new TryPlayCardAction(player, card);

            Container.Perform(action);
        }

        private void OnValidateTryCardPlay(object sender, object args)
        {
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

            if (action.Card.Space == Shared.Space.None)
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

            if (Container.GetMatch().Players[action.Card.OwnerIndex].AgentCount == 0)
            {
                LogUtility.LogWarning<PlayerSystem>("Agent action count is 0", colorName: ColorCodes.Logic);
                validator.Invalidate();
                return;
            }
        }

        private void OnPerformTryCardPlay(object sender, object args)
        {
            var tryAction = args as TryPlayCardAction;

            var playAction = new CardPlayAction(tryAction.Player, tryAction.Card);
            Container.Perform(playAction);
        }
        #endregion

        #region Card Play Action
        private void OnValidateCardPlay(object sender, object args)
        {
            // var action = sender as CardPlayAction;
            // var validator = args as Validator;
        }

        private void OnPerformCardPlay(object sender, object args)
        {
            var action = args as CardPlayAction;

            ChangeZone(action.Card, Zone.Agent);
            // Resource 지불
            PayAgentCount(Container.GetMatch().Players[action.Card.OwnerIndex]);
            // Tile로부터 Reward 받기

            action.Card.TryGetAspect(out Target target);
            LogUtility.Log<PlayerSystem>($"Player {action.Card.OwnerIndex} played {action.Card.Name} to {target.Selected.Name}", colorName: ColorCodes.Logic);
        }
        #endregion

        private void ShuffleDeck(Player player)
        {
            var random = new System.Random();
            var list = player[Zone.Deck];

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1); // 0부터 n까지의 랜덤 인덱스 선택
                (list[n], list[k]) = (list[k], list[n]); // Swap 연산 (C# 튜플)
            }
        }

        private Card Draw(Player player)
        {
            if (player[Zone.Deck].Count == 0)
            {
                for (int i = player[Zone.Graveyard].Count - 1; i >= 0; i--)
                {
                    var discard = player[Zone.Graveyard][i];
                    ChangeZone(discard, Zone.Deck);
                }

                ShuffleDeck(player);
            }

            int index = player[Zone.Deck].Count - 1;
            var result = player[Zone.Deck][index];
            ChangeZone(result, Zone.Hand);

            return result;
        }

        private List<Card> Draw(Player player, int amount)
        {
            int resultAmount = Mathf.Min(amount, player[Zone.Deck].Count + player[Zone.Graveyard].Count);
            List<Card> result = new(resultAmount);

            for (int i = 0; i < resultAmount; i++)
            {
                var card = Draw(player);
                result.Add(card);
            }

            return result;
        }

        private void PayAgentCount(Player player)
        {
            if (player.TurnActionCount == 0)
            {
                LogUtility.LogWarning<PlayerSystem>("Turn action count가 0입니다.");
                return;
            }

            if (player.AgentCount == 0)
            {
                LogUtility.LogWarning<PlayerSystem>("Agent Count가 0입니다.");
                return;
            }

            player.TurnActionCount--;
            player.AgentCount--;
        }

        private void RestoreTurnActionCount(Player player)
        {
            player.TurnActionCount = Player.InitialTurnActionCount;
        }

        private void ChangeZone(Card card, Zone zone, Player toPlayer = null)
        {
            if (!Container.TryGetAspect<CardSystem>(out var cardSystem)) return;
            cardSystem.ChangeZone(card, zone, toPlayer);
        }
    }
}