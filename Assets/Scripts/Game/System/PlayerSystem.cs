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
            this.AddObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>(), Container);
            this.AddObserver(OnPerformRoundStart, Global.PerformNotification<RoundStartAction>(), Container);
            this.AddObserver(OnPerformCardsDraw, Global.PerformNotification<CardsDrawAction>(), Container);
            this.AddObserver(OnValidateCardPlay, Global.ValidationNotification<CardPlayAction>());
            this.AddObserver(OnPerformCardPlay, Global.PerformNotification<CardPlayAction>(), Container);
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>(), Container);
            this.RemoveObserver(OnPerformRoundStart, Global.PerformNotification<RoundStartAction>(), Container);
            this.RemoveObserver(OnPerformCardsDraw, Global.PerformNotification<CardsDrawAction>(), Container);
            this.RemoveObserver(OnValidateCardPlay, Global.ValidationNotification<CardPlayAction>());
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
                var action = new CardsDrawAction(player, Player.InitialHand);
                Container.AddReaction(action);
            }
        }

        private void OnPerformCardsDraw(object sender, object args)
        {
            var action = args as CardsDrawAction;

            action.Cards = Draw(action.Player, action.Amount);
        }

        private void OnValidateCardPlay(object sender, object args)
        {
            var action = sender as CardPlayAction;
            var validator = args as Validator;

            if (action.Card.OwnerIndex != Container.GetMatch().CurrentPlayerIndex)
                validator.Invalidate();

            if (action.Card.Zone != Zone.Hand)
                validator.Invalidate();

            if (Container.GetMatch().Players[action.Card.OwnerIndex].AgentCount == 0)
            {
                validator.Invalidate();
            }
        }

        private void OnPerformCardPlay(object sender, object args)
        {
            var action = args as CardPlayAction;

            PayAgentCount(Container.GetMatch().Players[action.Card.OwnerIndex]);
            ChangeZone(action.Card, Zone.Agent);
        }

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
            if (player.AgentCount == 0)
            {
                LogUtility.LogWarning<PlayerSystem>("Agent Count가 0입니다.");
                return;
            }

            player.AgentCount--;
        }

        private void ChangeZone(Card card, Zone zone, Player toPlayer = null)
        {
            if (!Container.TryGetAspect<CardSystem>(out var cardSystem)) return;
            cardSystem.ChangeZone(card, zone, toPlayer);
        }
    }
}