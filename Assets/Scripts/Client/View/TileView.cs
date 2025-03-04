using TMPro;
using CCGP.Shared;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CCGP.Client
{
    public class TileView : MonoBehaviour, IFSMHandler
    {
        #region Color value
        public string Color_Lunar = "#B9B9B9";
        public string Color_Marsion = "#FF952F";
        public string Color_Water = "#A0F3FF";

        public string Color_Emperor = "#E0E0E0";
        public string Color_SpacingGuild = "#E02300";
        public string Color_BeneGesserit = "#8100E9";
        public string Color_Fremen = "#3D6BFF";
        #endregion

        public SerializedTile Data { get; private set; }

        public IMouseInput Input { get; private set; }
        public string Name => Text_Name.text;

        [Header("Basic")]
        public TMP_Text Text_Name;
        public TMP_Text Text_ID;
        public Image Icon;
        public GameObject BlockImage;
        public List<Sprite> IconSprites;

        [Header("Condition")]
        public GameObject Panel_Condition;
        public Image Image_Condition;
        public TMP_Text Text_Condition;

        [Header("Cost")]
        public GameObject Panel_Cost;
        public Image Image_Cost;
        public TMP_Text Text_Cost;

        [Header("Reward")]
        public GameObject Panel_Reward;
        public Image Image_Reward;
        public TMP_Text Text_Reward;

        [Header("Battle")]
        public GameObject Panel_Battle;

        [Header("Agent")]
        public GameObject Object_Agent;

        [Header("Info")]
        public GameObject Object_Info;
        public TMP_Text Text_Info;

        public void UpdateData(SerializedTile tile)
        {
            Data = tile;

            Text_Name.text = tile.Name;
            Text_Info.text = $"{tile.Description}";
            Text_ID.gameObject.SetActive(false);

            if (tile.AgentIndex != -1)
            {
                Object_Agent.SetActive(true);
            }
            else
            {
                Object_Agent.SetActive(false);
            }

            switch (tile.Space)
            {
                case Shared.Space.Yellow:
                    Icon.sprite = IconSprites[6];
                    break;
                case Shared.Space.Green:
                    Icon.sprite = IconSprites[5];
                    break;
                case Shared.Space.Blue:
                    Icon.sprite = IconSprites[4];
                    break;
                case Shared.Space.Emperor:
                    // Icon.sprite = IconSprites[3];
                    Icon.gameObject.SetActive(false);
                    break;
                case Shared.Space.SpacingGuild:
                    // Icon.sprite = IconSprites[2];
                    Icon.gameObject.SetActive(false);
                    break;
                case Shared.Space.BeneGesserit:
                    // Icon.sprite = IconSprites[1];
                    Icon.gameObject.SetActive(false);
                    break;
                case Shared.Space.Fremen:
                    // Icon.sprite = IconSprites[0];
                    Icon.gameObject.SetActive(false);
                    break;
            }

            switch (tile.CostType)
            {
                case ResourceType.None:
                    Panel_Cost.SetActive(false);
                    break;
                case ResourceType.Lunar:
                    Panel_Cost.SetActive(true);
                    ColorUtility.TryParseHtmlString(Color_Lunar, out var color);
                    Image_Cost.color = color;
                    Text_Cost.text = tile.CostAmount.ToString();
                    break;
                case ResourceType.Marsion:
                    Panel_Cost.SetActive(true);
                    ColorUtility.TryParseHtmlString(Color_Marsion, out color);
                    Image_Cost.color = color;
                    Text_Cost.text = tile.CostAmount.ToString();
                    break;
                case ResourceType.Water:
                    Panel_Cost.SetActive(true);
                    ColorUtility.TryParseHtmlString(Color_Water, out color);
                    Image_Cost.color = color;
                    Text_Cost.text = tile.CostAmount.ToString();
                    break;
            }

            switch (tile.ConditionType)
            {
                case ConditionType.None:
                    Panel_Condition.SetActive(false);
                    break;
                case ConditionType.Emperor:
                    Panel_Condition.SetActive(true);
                    ColorUtility.TryParseHtmlString(Color_Emperor, out var color);
                    Image_Condition.color = color;
                    Text_Condition.text = tile.ConditionAmount.ToString();
                    break;
                case ConditionType.SpacingGuild:
                    Panel_Condition.SetActive(true);
                    ColorUtility.TryParseHtmlString(Color_SpacingGuild, out color);
                    Image_Condition.color = color;
                    Text_Condition.text = tile.ConditionAmount.ToString();
                    break;
                case ConditionType.BeneGesserit:
                    Panel_Condition.SetActive(true);
                    ColorUtility.TryParseHtmlString(Color_BeneGesserit, out color);
                    Image_Condition.color = color;
                    Text_Condition.text = tile.ConditionAmount.ToString();
                    break;
                case ConditionType.Fremen:
                    Panel_Condition.SetActive(true);
                    ColorUtility.TryParseHtmlString(Color_Fremen, out color);
                    Image_Condition.color = color;
                    Text_Condition.text = tile.ConditionAmount.ToString();
                    break;
            }
        }

        public void OnPointerEnter()
        {
            Object_Info.SetActive(true);

            var canvas = Object_Info.AddComponent<Canvas>();
            var scaler = Object_Info.AddComponent<CanvasScaler>();

            canvas.overrideSorting = true;
            canvas.sortingOrder = 100;

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0f;
        }

        public void OnPointerExit()
        {
            Object_Info.SetActive(false);

            var scaler = Object_Info.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                DestroyImmediate(scaler);
            }

            var canvas = Object_Info.GetComponent<Canvas>();
            if (canvas != null)
            {
                DestroyImmediate(canvas);
            }
        }
    }
}