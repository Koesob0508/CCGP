using CCGP.AspectContainer;
using CCGP.Shared;
using System.Collections;
using System.Collections.Generic;

namespace CCGP.Server
{
    public class TargetSystem : Aspect, IActivatable
    {
        public void Activate()
        {
            this.AddObserver(OnValidateCardPlayAction, Global.ValidationNotification(typeof(CardPlayAction)));
            this.AddObserver(OnSelectTile, Global.MessageNotification(GameCommand.TrySelectTile));
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnSelectTile, Global.MessageNotification(GameCommand.TrySelectTile));
            this.RemoveObserver(OnValidateCardPlayAction, Global.ValidationNotification(typeof(CardPlayAction)));
        }

        private void OnValidateCardPlayAction(object sender, object args)
        {
            var action = sender as CardPlayAction;
            var validator = args as Validator;
            if (action != null && validator.IsValid)
            {
                action.PreparePhase.Awaiter = WaitTargetSelect;
            }
        }

        private void OnSelectTile(object sender, object args)
        {
            var sData = args as SerializedData;
            var sTile = sData.Get<SerializedTile>();

            Container.TryGetAspect<ActionSystem>(out var actionSystem);
            var action = actionSystem.CurrentAction as CardPlayAction;

            if (action != null)
            {
                SetTarget(action, new Tile(sTile));
            }
        }

        private IEnumerator WaitTargetSelect(IContainer game, GameAction action)
        {
            var cardPlayAction = action as CardPlayAction;
            if (cardPlayAction == null) yield break;

            if (!cardPlayAction.Card.TryGetAspect(out Target target))
            {
                yield return true;
                yield break;
            }

            // 가능한 지역 목록 메시징
            var targetTiles = GetTile(cardPlayAction.Card);

            var log = "보낼 수 있는 지역 : \n";
            for (int i = 0; i < targetTiles.Count; i++)
            {
                log += $"{i + 1}. {targetTiles[i].Name} ";
            }
            LogUtility.Log<TargetSystem>(log, colorName: ColorCodes.Logic);
            Container.PostNotification(Global.MessageNotification(GameCommand.ShowAvailableTiles), targetTiles);

            while (target.Selected == null)
            {
                yield return false;
            }

            // Selected와 Card의 Space 비교
            if (cardPlayAction.Card.Space.Contains(target.Selected.Space))
            {
                // SendAgent를 해야하나?
                // 아니 어차피 Card Target이 정해졌으니까 나중에 알아서 처리하면 된다.
                // 유효한 타겟이 선택됨, 이후 로직에서 처리됨
                LogUtility.Log<TargetSystem>($"선택된 타겟: {target.Selected.Name}");
            }
            else
            {
                // 만일 잘못된 경우, action.Cancel()을 낸다.
                LogUtility.LogWarning<TargetSystem>("잘못된 타겟이 선택됨. 카드 발동이 취소됩니다.");
                action.Cancel();
                target.Selected = null;
            }

            yield return true;
        }

        // 외부(클라이언트 또는 다른 시스템)에서 타겟 선택 입력을 받아, 
        // 해당 CardPlayAction의 Target 컴포넌트에 선택된 타일을 설정하는 메서드.
        public void SetTarget(CardPlayAction action, Tile selectedTile)
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

        private List<Tile> GetTile(Card source)
        {
            var result = new List<Tile>();
            // 보드에서 타일들을 뒤진다.
            foreach (var tile in Container.GetMatch().Board.Tiles)
            {
                // Card.Space와 같으면서 소드마스터가 없는 타일을 찾는다.
                if (source.Space.Contains(tile.Space) && tile.AgentIndex == -1)
                {
                    result.Add(tile);
                }
            }

            return result;
        }
    }
}