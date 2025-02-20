using TMPro;
using CCGP.Shared;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CCGP.Client
{
    public class TileView : MonoBehaviour
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
        public TMP_Text Name;
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

        public void UpdateView(SerializedTile tile)
        {
            Data = tile;

            Name.text = tile.Name;

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
                    Icon.sprite = IconSprites[3];
                    break;
                case Shared.Space.SpacingGuild:
                    Icon.sprite = IconSprites[2];
                    break;
                case Shared.Space.BeneGesserit:
                    Icon.sprite = IconSprites[1];
                    break;
                case Shared.Space.Fremen:
                    Icon.sprite = IconSprites[0];
                    break;
            }

            switch (tile.CostType)
            {
                case CostType.None:
                    Panel_Cost.SetActive(false);
                    break;
                case CostType.Lunar:
                    Panel_Cost.SetActive(true);
                    ColorUtility.TryParseHtmlString(Color_Lunar, out var color);
                    Image_Cost.color = color;
                    LogUtility.LogWarning<TileView>($"Cost : {color}");
                    Text_Cost.text = tile.CostAmount.ToString();
                    break;
                case CostType.Marsion:
                    Panel_Cost.SetActive(true);
                    ColorUtility.TryParseHtmlString(Color_Marsion, out color);
                    Image_Cost.color = color;
                    LogUtility.LogWarning<TileView>($"Cost : {color}");
                    Text_Cost.text = tile.CostAmount.ToString();
                    break;
                case CostType.Water:
                    Panel_Cost.SetActive(true);
                    ColorUtility.TryParseHtmlString(Color_Water, out color);
                    Image_Cost.color = color;
                    LogUtility.LogWarning<TileView>($"Cost : {color}");
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
                    LogUtility.LogWarning<TileView>($"Condition : {color}");
                    Text_Condition.text = tile.ConditionAmount.ToString();
                    break;
                case ConditionType.SpacingGuild:
                    Panel_Condition.SetActive(true);
                    ColorUtility.TryParseHtmlString(Color_SpacingGuild, out color);
                    Image_Condition.color = color;
                    LogUtility.LogWarning<TileView>($"Condition : {color}");
                    Text_Condition.text = tile.ConditionAmount.ToString();
                    break;
                case ConditionType.BeneGesserit:
                    Panel_Condition.SetActive(true);
                    ColorUtility.TryParseHtmlString(Color_BeneGesserit, out color);
                    Image_Condition.color = color;
                    LogUtility.LogWarning<TileView>($"Condition : {color}");
                    Text_Condition.text = tile.ConditionAmount.ToString();
                    break;
                case ConditionType.Fremen:
                    Panel_Condition.SetActive(true);
                    ColorUtility.TryParseHtmlString(Color_Fremen, out color);
                    Image_Condition.color = color;
                    LogUtility.LogWarning<TileView>($"Condition : {color}");
                    Text_Condition.text = tile.ConditionAmount.ToString();
                    break;
            }
        }
    }
}
