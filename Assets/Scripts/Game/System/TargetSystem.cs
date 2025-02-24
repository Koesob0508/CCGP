using CCGP.AspectContainer;
using CCGP.Shared;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CCGP.Server
{
    public class TargetSystem : Aspect, IActivatable
    {
        private GameAction Action;

        public void Activate()
        {
            this.AddObserver(OnValidateTryCardPlay, Global.ValidateNotification<TryPlayCardAction>());

            this.AddObserver(OnValidateCardPlay, Global.ValidateNotification<PlayCardAction>());

            this.AddObserver(OnCancelTryPlayCard, Global.MessageNotification(GameCommand.TryCancelPlayCard));
            this.AddObserver(OnCancelPlayCard, Global.CancelNotification<PlayCardAction>());

            this.AddObserver(OnReceivedSelectTile, Global.MessageNotification(GameCommand.TrySelectTile));
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnValidateTryCardPlay, Global.ValidateNotification<TryPlayCardAction>());

            this.RemoveObserver(OnValidateCardPlay, Global.ValidateNotification<PlayCardAction>());

            this.RemoveObserver(OnCancelTryPlayCard, Global.MessageNotification(GameCommand.TryCancelPlayCard));
            this.RemoveObserver(OnCancelPlayCard, Global.CancelNotification<PlayCardAction>());

            this.RemoveObserver(OnReceivedSelectTile, Global.MessageNotification(GameCommand.TrySelectTile));
        }

        private void OnValidateTryCardPlay(object sender, object args)
        {
            var action = sender as TryPlayCardAction;

            var card = action.Card;

            if (card.TryGetAspect<Target>(out var target))
            {
                action.PreparePhase.Awaiter = WaitTargetSelect;
            }
        }
        private void OnValidateCardPlay(object sender, object args)
        {
            var action = sender as PlayCardAction;
            var validator = args as Validator;

            // 앞선 시스템(== Target System)에서 이미 불합격이라면 검증 안해도 됨
            if (!validator.IsValid) return;

            var card = action.Card;
            if (card.TryGetAspect<Target>(out var target))
            {
                if (target.Selected == null)
                {
                    // target 특성 있는데 selected가 없는 경우 invalidate
                    validator.Invalidate();
                    return;
                }
            }

            // 검증은 별도로 진행
            // Selected와 Card의 Space 비교
            if (action.Card.Space.Contains(target.Selected.Space))
            {
                // SendAgent를 해야하나?
                // 아니 어차피 Card Target이 정해졌으니까 나중에 알아서 처리하면 된다.
                // 유효한 타겟이 선택됨, 이후 로직에서 처리됨
                LogUtility.Log<TargetSystem>($"조건 만족. 선택된 타겟: {target.Selected.Name}");
            }
            else
            {
                // 만일 잘못된 경우, action.Cancel()을 낸다.
                LogUtility.LogWarning<TargetSystem>("잘못된 타겟이 선택됨. 카드 발동이 취소됩니다.");
                validator.Invalidate();
                target.Selected = null;
            }
        }

        private void OnCancelPlayCard(object sender, object args)
        {
            var action = args as PlayCardAction;
            if (action.Card.TryGetAspect<Target>(out var target))
            {
                target.Selected = null;
            }
        }

        private void OnReceivedSelectTile(object sender, object args)
        {
            var sData = args as SerializedData;
            var sTile = sData.Get<SerializedTile>();

            var tryPlayAction = Action as TryPlayCardAction;

            if (tryPlayAction != null)
            {
                SetTarget(tryPlayAction, GetTile(sTile));
            }
        }

        private void OnCancelTryPlayCard(object sender, object args)
        {
            if (Action != null)
            {
                Action.Cancel();
            }
        }

        private IEnumerator WaitTargetSelect(IContainer game, GameAction action)
        {
            var tryPlayCardAction = action as TryPlayCardAction;
            if (tryPlayCardAction == null) yield break;

            // 두 번 검증해버리네
            if (!tryPlayCardAction.Card.TryGetAspect(out Target target))
            {
                yield return true;
                yield break;
            }

            Action = tryPlayCardAction;
            // 가능한 지역 목록 메시징
            var targetTiles = GetAvailableTile(tryPlayCardAction.Card, tryPlayCardAction.Player);

            var log = "보낼 수 있는 지역 : \n";
            for (int i = 0; i < targetTiles.Count; i++)
            {
                log += $"{i + 1}. {targetTiles[i].Name} ";
            }
            LogUtility.Log<TargetSystem>(log, colorName: ColorCodes.Logic);
            Container.PostNotification(Global.MessageNotification(GameCommand.ShowAvailableTiles), new Tuple<Player, List<Tile>>(tryPlayCardAction.Player, targetTiles));

            while (target.Selected == null && !action.IsCanceled)
            {
                yield return false;
            }

            if (action.IsCanceled)
            {
                LogUtility.LogWarning<TargetSystem>("TryPlayCardAction이 취소되었습니다.", colorName: ColorCodes.Logic);
            }
            else
            {
                // target.Selected 됨 
                LogUtility.Log<TargetSystem>($"Wait Target Select 끝. 선택된 타겟: {target.Selected.Name}");
            }

            Action = null;
            yield return true;
        }

        // 외부(클라이언트 또는 다른 시스템)에서 타겟 선택 입력을 받아, 
        // 해당 CardPlayAction의 Target 컴포넌트에 선택된 타일을 설정하는 메서드.
        public void SetTarget(TryPlayCardAction action, Tile selectedTile)
        {
            if (action == null || selectedTile == null)
            {
                LogUtility.LogWarning<TargetSystem>("Action이나 selectedTile이 null입니다.");
                return;
            }

            if (action.Card.TryGetAspect(out Target target))
            {
                target.Selected = selectedTile;
                LogUtility.Log<TargetSystem>($"타겟이 '{selectedTile.Name}'으로 설정되었습니다.");
            }
            else
            {
                LogUtility.LogWarning<TargetSystem>("해당 카드에 Target Aspect가 없습니다.");
            }
        }

        private Tile GetTile(SerializedTile sTile)
        {
            foreach (var tile in Container.GetMatch().Board.Tiles)
            {
                if (tile.Name == sTile.Name)
                {
                    return tile;
                }
            }

            LogUtility.LogWarning<TargetSystem>("해당 타일이 존재하지 않는다고?", colorName: ColorCodes.Logic);
            return null;
        }

        private List<Tile> GetAvailableTile(Card source, Player player)
        {
            var result = new List<Tile>();
            // 보드에서 타일들을 뒤진다.
            foreach (var tile in Container.GetMatch().Board.Tiles)
            {
                // Card.Space와 같으면서 소드마스터가 없는 타일을 찾는다.
                if (source.Space.Contains(tile.Space) && tile.AgentIndex == -1)
                {
                    // 조건을 확인해서 조건에 통과하는지 확인한다.
                    if (tile.TryGetAspect<Condition>(out var condition))
                    {
                        // 넘어갑니다.
                    }
                    result.Add(tile);
                }
            }

            return result;
        }
    }
}