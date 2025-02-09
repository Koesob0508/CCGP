using UnityEngine;

namespace CCGP.Client
{
    public class GameViewManager : MonoBehaviour
    {
        public GameObject GamePanel;

        public void Start()
        {
            this.AddObserver(OnGameStart, "StartGame");
        }

        private void OnGameStart(object sender, object args)
        {
            GamePanel.SetActive(true);
        }
    }
}