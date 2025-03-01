using CCGP.Shared;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CCGP.Client
{
    public class CardView : MonoBehaviour, IFSMHandler
    {
        public SerializedCard Data { get; private set; }

        public TMP_Text Text_Name;
        public TMP_Text Text_Cost;
        public TMP_Text Text_Persuasion;
        public GameObject Root_Space;
        public GameObject Prefab_SpaceIcon;
        public GameObject HoverImage;
        public List<Sprite> Sprite_Icons;

        public float HoverHeight = 20f;

        public Transform Transform { get; protected set; }
        public Collider2D Collider { get; protected set; }
        public IMouseInput Input { get; protected set; }
        public CardViewFSM FSM { get; protected set; }

        public string Name => Text_Name.text;

        #region Unity Callbacks

        private void Awake()
        {
            Transform = GetComponent<Transform>();
            Collider = GetComponent<Collider2D>();
            Input = GetComponent<IMouseInput>();
            FSM = new CardViewFSM(this);
        }

        private void Update()
        {
            FSM?.Update();
        }

        #endregion

        public void Enable()
        {
            FSM.PushState<CardViewIdle>();
        }

        public void UpdateData(SerializedCard card)
        {
            Data = card;

            gameObject.name = $"{card.Name}-{card.GUID.Substring(0, 4)}";

            Text_Name.text = card.Name;
            Text_Cost.text = card.Cost.ToString();
            Text_Persuasion.text = card.Persuasion.ToString();

            SetSpace();
        }

        public void Draw() => FSM.PushState<CardViewDraw>();

        private void SetSpace()
        {
            if (Data.Space.Contains(Shared.Space.Yellow))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[6];
            }

            if (Data.Space.Contains(Shared.Space.Green))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[5];
            }

            if (Data.Space.Contains(Shared.Space.Blue))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[4];
            }

            if (Data.Space.Contains(Shared.Space.Emperor))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[3];
            }

            if (Data.Space.Contains(Shared.Space.SpacingGuild))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[2];
            }
            if (Data.Space.Contains(Shared.Space.BeneGesserit))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[1];
            }
            if (Data.Space.Contains(Shared.Space.Fremen))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[0];
            }
        }
    }
}