using CCGP.Shared;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CCGP.Client
{
    public class CardViewHover : BaseCardViewState
    {
        public CardViewHover(CardView handler, BaseStateMachine fsm) : base(handler, fsm) { }

        #region State Operations
        public override void OnEnter()
        {
            Handler.Input.OnPointerExit -= OnPointerExit;
            Handler.Input.OnPointerExit += OnPointerExit;

            Handler.Input.OnPointerClick -= OnPointerClick;
            Handler.Input.OnPointerClick += OnPointerClick;

            //Handler.Input.OnDrag -= OnDrag;
            //Handler.Input.OnDrag += OnDrag;

            SetScale();
            SetPosition();
            SetRotation();
            SetOrder();
        }

        public override void OnExit()
        {
            Handler.HoverImage.transform.localPosition = Vector3.zero;
            Handler.HoverImage.transform.localRotation = Quaternion.identity;
            Handler.Input.OnPointerExit -= OnPointerExit;
            Handler.Input.OnPointerClick -= OnPointerClick;
            //Handler.Input.OnDrag -= OnDrag;

            var raycaster = Handler.gameObject.GetComponent<GraphicRaycaster>();
            if (raycaster != null)
                Object.DestroyImmediate(raycaster);

            var scaler = Handler.gameObject.GetComponent<CanvasScaler>();
            if (scaler != null)
                Object.DestroyImmediate(scaler);

            var canvas = Handler.gameObject.GetComponent<Canvas>();
            if (canvas != null)
                Object.DestroyImmediate(canvas);
        }
        #endregion

        #region Pointer Operations
        private void OnPointerExit(PointerEventData eventData)
        {
            if (FSM.IsCurrent(this))
            {
                // 이전 상태가 Idle 상태이기 때문에 PopState
                FSM.PopState();
            }
        }

        private void OnPointerClick(PointerEventData eventData)
        {
            if (FSM.IsCurrent(this) && eventData.button == PointerEventData.InputButton.Left)
            {
                var match = Handler.GetComponentInParent<MatchView>();

                if (match.Data.CurrentPlayerIndex == Handler.Data.OwnerIndex)
                {
                    FSM.PopState();

                    FSM.PushState<CardViewSelect>();
                }
                else
                {
                    // 나중에 대사로 출력하세요.
                    LogUtility.Log<CardViewHover>("당신의 턴이 아닙니다.", colorName: "red");
                    FSM.PopState();
                }
            }
        }

        //private void OnDrag(PointerEventData eventData)
        //{
        //    if (FSM.IsCurrent(this) && eventData.button == PointerEventData.InputButton.Left)
        //    {
        //        //if (Managers.Instance.Client.Game.IsMyTurn())
        //        if (false)
        //        {
        //            FSM.PopState();

        //            FSM.PushState<CardViewDrag>();
        //        }
        //        else
        //        {
        //            LogUtility.Log<CardViewHover>("그렇게는 할 수 없어요.", colorName: "red");
        //            FSM.PopState();
        //        }
        //    }
        //}
        #endregion

        #region Utils

        private void SetScale()
        {
            var currentScale = Handler.Transform.localScale;
            var finalScale = currentScale * 2;

            Handler.HoverImage.transform.localScale = finalScale;
        }

        private void SetPosition()
        {
            var finalPosition = Handler.Transform.position + new Vector3(0, Handler.HoverHeight, -2f);

            //Handler.MoveToWithZ(finalPosition, Parameters.HoverSpeed);
            Handler.HoverImage.transform.position = finalPosition;
        }

        private void SetRotation()
        {
            //Handler.Rotation.StopMotion();
            Handler.HoverImage.transform.rotation = Quaternion.identity;
        }

        private void SetOrder()
        {
            var canvas = Handler.gameObject.AddComponent<Canvas>();
            var scaler = Handler.gameObject.AddComponent<CanvasScaler>();
            var raycaster = Handler.gameObject.AddComponent<GraphicRaycaster>();

            canvas.overrideSorting = true;
            canvas.sortingOrder = 100;

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0f;
        }

        #endregion
    }
}
