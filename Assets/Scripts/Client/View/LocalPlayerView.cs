using CCGP.Shared;
using TMPro;
using UnityEngine;

namespace CCGP.Client
{
    public class LocalPlayerView : BaseView
    {
        [Header("VP")]
        public TMP_Text Text_VictoryPoint;

        [Header("Resources")]
        public TMP_Text Text_Agent;
        public TMP_Text Text_Marsion;
        public TMP_Text Text_Lunar;
        public TMP_Text Text_Water;
        public TMP_Text Text_Troop;

        [Header("Influence")]
        public TMP_Text Text_Emperor;
        public TMP_Text Text_Guild;
        public TMP_Text Text_Coven;
        public TMP_Text Text_Fremen;

        public override void Activate()
        {
            LogUtility.Log<LocalPlayerView>("HI");
        }

        public override void Deactivate()
        {

        }

        private void OnUpdateData(object sender, object args)
        {

        }
    }
}