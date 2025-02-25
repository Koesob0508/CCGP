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
            this.AddObserver(OnPerformStartGame, Global.PerformNotification<StartGameAction>(), Container);
            this.AddObserver(OnPerformStartRound, Global.PerformNotification<StartRoundAction>(), Container);
            this.AddObserver(OnPerformStartTurn, Global.PerformNotification<StartTurnAction>(), Container);

            // Client message notify
            this.AddObserver(OnReceivedTryPlayCard, Global.MessageNotification(GameCommand.TryPlayCard), Container);
            this.AddObserver(OnReceivedTryRevealCards, Global.MessageNotification(GameCommand.TryOpenCards), Container);

            // Player Game Action
            this.AddObserver(OnPerformRoundStartDraw, Global.PerformNotification<RoundStartDrawAction>(), Container);
            this.AddObserver(OnPerformDrawCards, Global.PerformNotification<DrawCardsAction>(), Container);
            this.AddObserver(OnPerformTryPlayCard, Global.PerformNotification<TryPlayCardAction>(), Container);
            this.AddObserver(OnPerformPlayCard, Global.PerformNotification<PlayCardAction>(), Container);
            this.AddObserver(OnPerformGainResources, Global.PerformNotification<GainResourcesAction>(), Container);
            this.AddObserver(OnPerformGenerateCard, Global.PerformNotification<GenerateCardAction>(), Container);
            this.AddObserver(OnPerformOpenCards, Global.PerformNotification<RevealCardsAction>(), Container);
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnPerformStartGame, Global.PerformNotification<StartGameAction>(), Container);
            this.RemoveObserver(OnPerformStartRound, Global.PerformNotification<StartRoundAction>(), Container);
            this.RemoveObserver(OnPerformStartTurn, Global.PerformNotification<StartTurnAction>(), Container);

            this.RemoveObserver(OnReceivedTryPlayCard, Global.MessageNotification(GameCommand.TryPlayCard), Container);

            this.RemoveObserver(OnPerformRoundStartDraw, Global.PerformNotification<RoundStartDrawAction>(), Container);
            this.RemoveObserver(OnPerformDrawCards, Global.PerformNotification<DrawCardsAction>(), Container);
            this.RemoveObserver(OnPerformTryPlayCard, Global.PerformNotification<TryPlayCardAction>(), Container);
            this.RemoveObserver(OnPerformPlayCard, Global.PerformNotification<PlayCardAction>(), Container);
            this.RemoveObserver(OnPerformGainResources, Global.PerformNotification<GainResourcesAction>(), Container);
            this.RemoveObserver(OnPerformGenerateCard, Global.PerformNotification<GenerateCardAction>(), Container);
        }

        private void OnPerformStartGame(object sender, object args)
        {
            var action = args as StartGameAction;
            foreach (var player in action.Match.Players)
            {
                ShuffleDeck(player);
            }
        }

        private void OnPerformStartRound(object sender, object args)
        {
            var batchAction = new RoundStartDrawAction();

            Container.AddReaction(batchAction);

            // foreach (var player in Container.GetMatch().Players)
            // {
            //     // TurnCount랑 AgentCount 회복 필요
            //     var action = new DrawCardsAction(player, Player.InitialHand);
            //     Container.AddReaction(action);
            // }
        }

        private void OnPerformRoundStartDraw(object sender, object args)
        {
            var action = args as RoundStartDrawAction;

            foreach (var player in Container.GetMatch().Players)
            {
                action[player.Index] = Draw(player, Player.InitialHand);
            }
        }

        private void OnPerformStartTurn(object sender, object args)
        {
            var action = args as StartTurnAction;

            var player = action.Match.Players[action.TargetPlayerIndex];

            RestoreTurnActionCount(player);
            player.IsOpened = false;
        }

        private void OnPerformDrawCards(object sender, object args)
        {
            var action = args as DrawCardsAction;

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

        private void OnReceivedTryRevealCards(object sender, object args)
        {
            LogUtility.Log<PlayerSystem>("Received OnTryRevealCard", colorName: ColorCodes.Logic);
            var sData = args as SerializedData;
            var sPlayer = sData.Get<SerializedPlayer>();

            var match = Container.GetMatch();
            var player = match.Players[sPlayer.Index];

            var action = new RevealCardsAction(player);
            Container.Perform(action);
        }

        private void OnPerformTryPlayCard(object sender, object args)
        {
            var tryAction = args as TryPlayCardAction;

            var playAction = new PlayCardAction(tryAction.Player, tryAction.Card);
            Container.Perform(playAction);
        }
        #endregion

        #region Card Play Action

        private void OnPerformPlayCard(object sender, object args)
        {
            var action = args as PlayCardAction;
            action.Card.TryGetAspect(out Target target);

            ChangeZone(action.Card, Zone.Agent);
            // Resource 지불
            if (target.Selected.TryGetAspect(out Cost cost))
            {
                PayCost(action.Player, cost);
            }
            PayAgentCount(action.Player);
            // Tile로부터 Reward 받기 -> AbilitySystem에 의해서 알아서 처리

            LogUtility.Log<PlayerSystem>($"Player {action.Card.OwnerIndex} played {action.Card.Name} to {target.Selected.Name}", colorName: ColorCodes.Logic);
        }
        #endregion

        private void OnPerformGainResources(object sender, object args)
        {
            var action = args as GainResourcesAction;

            var player = action.Player;
            var type = action.Type;
            var amount = action.Amount;

            switch (type)
            {
                case ResourceType.Water:
                    player.Water += amount;
                    break;
                case ResourceType.Lunar:
                    player.Lunar += amount;
                    break;
                case ResourceType.Marsion:
                    player.Marsion += amount;
                    break;
                case ResourceType.Troop:
                    player.Troop += amount;
                    break;
                case ResourceType.Persuasion:
                    player.Persuasion += amount;
                    break;
                case ResourceType.BasePersuasion:
                    player.BasePersuasion += amount;
                    break;
                case ResourceType.Mentat:
                    player.MentatCount += amount;
                    break;
                case ResourceType.BaseAgent:
                    player.BaseAgentCount += amount;
                    break;
                case ResourceType.VictoryPoint:
                    player.VictoryPoint += amount;
                    break;
                default:
                    LogUtility.LogWarning<PlayerSystem>($"Unknown resource type: {type}");
                    break;
            }
        }

        private void OnPerformGenerateCard(object sender, object args)
        {
            var action = args as GenerateCardAction;

            var player = action.Player;
            var card = action.Card;

            ChangeZone(card, Zone.Hand, player);
        }

        private void OnPerformOpenCards(object sender, object args)
        {
            var action = args as RevealCardsAction;

            var player = action.Player;

            var handCopy = new List<Card>();
            foreach (var handCard in player[Zone.Hand])
            {

                handCopy.Add(handCard);
            }

            foreach (var handCard in handCopy)
            {
                ChangeZone(handCard, Zone.Open);
            }

            action.Cards = handCopy;

            player.IsOpened = true;
        }

        #region Utility

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

        public List<Card> Draw(Player player, uint amount)
        {
            int resultAmount = (int)Mathf.Min(amount, player[Zone.Deck].Count + player[Zone.Graveyard].Count);
            List<Card> result = new(resultAmount);

            for (int i = 0; i < resultAmount; i++)
            {
                var card = Draw(player);
                result.Add(card);
            }

            return result;
        }

        private void PayCost(Player player, Cost cost)
        {
            LogUtility.Log<PlayerSystem>($"Pay Cost");
            switch (cost.Type)
            {
                case ResourceType.Water:
                    player.Water -= cost.Amount;
                    LogUtility.Log<PlayerSystem>($"Water:{player.Water}");
                    break;
                case ResourceType.Lunar:
                    player.Lunar -= cost.Amount;
                    LogUtility.Log<PlayerSystem>($"Water:{player.Lunar}");
                    break;
                case ResourceType.Marsion:
                    player.Marsion -= cost.Amount;
                    LogUtility.Log<PlayerSystem>($"Water:{player.Marsion}");
                    break;
                default:
                    LogUtility.LogWarning<PlayerSystem>($"Unknown resource type: {cost.Type}");
                    break;
            }
        }

        private void PayAgentCount(Player player)
        {
            if (player.TurnActionCount == 0)
            {
                LogUtility.LogWarning<PlayerSystem>("Turn action count가 0입니다.");
                return;
            }

            if (player.UsedAgentCount == player.TotalAgentCount)
            {
                LogUtility.LogWarning<PlayerSystem>("Agent Count가 0입니다.");
                return;
            }

            player.TurnActionCount--;
            player.UsedAgentCount++;
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

        #endregion
    }
}