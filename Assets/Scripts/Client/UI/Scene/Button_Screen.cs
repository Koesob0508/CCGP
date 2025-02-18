using TMPro;
using UnityEngine;

namespace CCGP.Client
{
    public class Button_Screen : MonoBehaviour
    {
        public TMP_Text ScreenModeText;

        private void Start()
        {
            if (Screen.fullScreen)
            {
                ScreenModeText.text = "작은 화면";
            }
            else
            {
                ScreenModeText.text = "전체 화면";
            }
        }

        public void OnClick()
        {
            SwitchScreen();
        }

        private void SwitchScreen()
        {
            if (Screen.fullScreen)
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
                ScreenModeText.text = "전체 화면";
            }
            else
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                ScreenModeText.text = "작은 화면";
            }
        }
    }
}