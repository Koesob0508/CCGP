using System.Collections.Generic;
using CCGP.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CCGP.Client
{
    public class ImperiumCardView : MonoBehaviour
    {
        public SerializedCard Data { get; private set; }

        public List<Sprite> Sprite_Icons;
        public GameObject Prefab_SpaceIcon;

        [Header("Base")]
        public GameObject CardImage;
        public TMP_Text Text_Name;
        public TMP_Text Text_Cost;
        public TMP_Text Text_Persuasion;
        public GameObject Root_Space;

        [Header("Hover")]
        public GameObject HoverImage;
        public TMP_Text Text_Hover_Name;
        public TMP_Text Text_Hover_Cost;
        public TMP_Text Text_Hover_Persuasion;
        public GameObject Root_Hover_Space;

        public void UpdateData(SerializedCard card)
        {
            Data = card;

            gameObject.name = $"{card.Name}-{card.GUID.Substring(0, 4)}";

            Text_Name.text = card.Name;
            Text_Cost.text = card.Cost.ToString();
            Text_Persuasion.text = card.Persuasion.ToString();

            Text_Hover_Name.text = card.Name;
            Text_Hover_Cost.text = card.Cost.ToString();
            Text_Hover_Persuasion.text = card.Persuasion.ToString();

            SetSpace();
        }

        public void OnPointerEnter()
        {
            HoverImage.SetActive(true);

            var canvas = HoverImage.AddComponent<Canvas>();
            var scaler = HoverImage.AddComponent<CanvasScaler>();

            canvas.overrideSorting = true;
            canvas.sortingOrder = 100;

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0f;
        }

        public void OnPointerExit()
        {
            HoverImage.SetActive(false);

            var scaler = HoverImage.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                DestroyImmediate(scaler);
            }

            var canvas = HoverImage.GetComponent<Canvas>();
            if (canvas != null)
            {
                DestroyImmediate(canvas);
            }
        }

        private void SetSpace()
        {
            if (Data.Space.Contains(Shared.Space.Yellow))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[6];

                var hoverIcon = Instantiate(Prefab_SpaceIcon, Root_Hover_Space.transform);
                hoverIcon.GetComponent<Image>().sprite = Sprite_Icons[6];
            }

            if (Data.Space.Contains(Shared.Space.Green))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[5];

                var hoverIcon = Instantiate(Prefab_SpaceIcon, Root_Hover_Space.transform);
                hoverIcon.GetComponent<Image>().sprite = Sprite_Icons[5];
            }

            if (Data.Space.Contains(Shared.Space.Blue))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[4];

                var hoverIcon = Instantiate(Prefab_SpaceIcon, Root_Hover_Space.transform);
                hoverIcon.GetComponent<Image>().sprite = Sprite_Icons[4];
            }

            if (Data.Space.Contains(Shared.Space.Emperor))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[3];

                var hoverIcon = Instantiate(Prefab_SpaceIcon, Root_Hover_Space.transform);
                hoverIcon.GetComponent<Image>().sprite = Sprite_Icons[3];
            }

            if (Data.Space.Contains(Shared.Space.SpacingGuild))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[2];

                var hoverIcon = Instantiate(Prefab_SpaceIcon, Root_Hover_Space.transform);
                hoverIcon.GetComponent<Image>().sprite = Sprite_Icons[2];
            }

            if (Data.Space.Contains(Shared.Space.BeneGesserit))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[1];

                var hoverIcon = Instantiate(Prefab_SpaceIcon, Root_Hover_Space.transform);
                hoverIcon.GetComponent<Image>().sprite = Sprite_Icons[1];
            }

            if (Data.Space.Contains(Shared.Space.Fremen))
            {
                var icon = Instantiate(Prefab_SpaceIcon, Root_Space.transform);
                icon.GetComponent<Image>().sprite = Sprite_Icons[0];

                var hoverIcon = Instantiate(Prefab_SpaceIcon, Root_Hover_Space.transform);
                hoverIcon.GetComponent<Image>().sprite = Sprite_Icons[0];
            }
        }
    }
}