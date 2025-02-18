using TMPro;
using CCGP.Shared;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CCGP.Client
{
    public class TileView : MonoBehaviour
    {
        public SerializedTile Data { get; private set; }
        public TMP_Text Name;
        public Image Icon;
        public GameObject BlockImage;
        public List<Sprite> IconSprites;

        public void Refresh(SerializedTile tile)
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
        }
    }
}