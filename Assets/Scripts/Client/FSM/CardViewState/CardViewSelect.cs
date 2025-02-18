using CCGP.Shared;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using CCGP.Server;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace CCGP.Client
{
    public class CardViewSelect : BaseCardViewState
    {
        PlayerView Player;

        public CardViewSelect(CardView handler, BaseStateMachine fsm) : base(handler, fsm) { }

        public override void OnInitialize()
        {
            Player = Handler.GetComponentInParent<PlayerView>();
        }

        public override void OnEnter()
        {
            Handler.transform.SetParent(Player.SelectedCard.transform);

            SetScale();
            SetPosition();
            SetOrder();

            Handler.Input.OnBeginDrag += OnBeginDrag;

            this.AddObserver(OnCancel, Global.MessageNotification(GameCommand.CancelPlayCard));
            this.PostNotification(Global.MessageNotification(GameCommand.TryPlayCard), Handler.Data);
        }

        public override void OnExit()
        {
            this.RemoveObserver(OnCancel, Global.MessageNotification(GameCommand.CancelPlayCard));

            Handler.Input.OnBeginDrag -= OnBeginDrag;

            UnsetOrder();

            Handler.transform.SetParent(Player.Hand.transform);
        }

        public override void OnUpdate()
        {
            if (Pointer.current.press.wasReleasedThisFrame)
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = Pointer.current.position.ReadValue();

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                foreach (RaycastResult result in results)
                {
                    var tileView = result.gameObject.GetComponent<TileView>();
                    if (tileView != null)
                    {
                        this.PostNotification(Global.MessageNotification(GameCommand.TrySelectTile), new SerializedTile() { Name = tileView.Data.Name, Space = tileView.Data.Space, AgentIndex = tileView.Data.AgentIndex });
                        FSM.PopState();
                        return;
                    }
                }
            }
        }

        #region Pointer Operations

        private void OnBeginDrag(PointerEventData eventData)
        {
            FSM.PopState();
            FSM.PushState<CardViewDrag>();
        }

        #endregion

        #region Private Methods

        private void SetOrder()
        {
            var canvas = Handler.gameObject.AddComponent<Canvas>();
            Handler.gameObject.AddComponent<GraphicRaycaster>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 100;
        }

        private void UnsetOrder()
        {
            var raycaster = Handler.gameObject.GetComponent<GraphicRaycaster>();
            if (raycaster != null)
                Object.Destroy(raycaster);

            var canvas = Handler.gameObject.GetComponent<Canvas>();
            if (canvas != null)
                Object.Destroy(canvas);
        }

        private void SetPosition()
        {
            Handler.transform.localPosition = Vector3.zero;
        }

        private void SetScale()
        {
            var currentScale = Handler.Transform.localScale;
            var finalScale = currentScale * 2;

            Handler.transform.localScale = finalScale;
        }

        private void OnCancel(object sender, object args)
        {
            if (FSM.IsCurrent(this))
                FSM.PopState();
            else
                LogUtility.LogWarning<CardViewSelect>($"Not current state Select. Current : {FSM.Current} / {Handler.gameObject.name}", colorName: ColorCodes.Logic);
        }

        #endregion
    }
}