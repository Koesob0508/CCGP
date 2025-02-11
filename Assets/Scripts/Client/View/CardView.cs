using CCGP.Shared;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CCGP.Client
{
    public class CardView : MonoBehaviour, IFSMHandler
    {
        public TMP_Text Text_Name;
        public TMP_Text Text_Cost;
        public TMP_Text Text_Persuasion;
        public GameObject Root_Space;
        public GameObject Prefab_SpaceIcon;
        public List<Sprite> Sprite_Icons;
        public GameObject HoverImage;


        public float HoverHeight = 20f;

        public bool IsMyTurn;

        public Transform Transform { get; private set; }
        public Collider2D Collider { get; private set; }
        public IMouseInput Input { get; private set; }
        public CardViewFSM FSM { get; private set; }

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

        public void Enable() => FSM.PushState<CardViewIdle>();

        public void Refresh(CardViewModel vm)
        {
            Text_Name.text = vm.Name;
            Text_Cost.text = vm.Cost.ToString();
            Text_Persuasion.text = vm.Persuasion.ToString();

            if(vm.Space.Contains(Shared.Space.Yellow))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[6];
            }

            if (vm.Space.Contains(Shared.Space.Green))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[5];
            }

            if (vm.Space.Contains(Shared.Space.Blue))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[4];
            }

            if (vm.Space.Contains(Shared.Space.Emperor))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[3];
            }

            if (vm.Space.Contains(Shared.Space.SpacingGuild))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[2];
            }
            if (vm.Space.Contains(Shared.Space.BeneGesserit))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[1];
            }
            if (vm.Space.Contains(Shared.Space.Fremen))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[0];
            }
        }

        public void Draw() => FSM.PushState<CardViewDraw>();
    }
}