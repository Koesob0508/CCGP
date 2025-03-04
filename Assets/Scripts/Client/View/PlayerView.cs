using CCGP.AspectContainer;
using CCGP.Server;
using CCGP.Shared;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace CCGP.Client
{
    public class PlayerView : BaseView
    {
        public SerializedPlayer ClientPlayer;

        public List<SerializedPlayer> Data { get; private set; }

        [Header("Prefab")]
        public GameObject Prefab_AgentView;

        [Header("Client Player")]
        public LocalPlayerView LocalPlayerView;
        public TMP_Text Text_VictoryPoint;
        public TMP_Text Text_Lunar;
        public TMP_Text Text_Marsion;
        public TMP_Text Text_Water;
        public TMP_Text Text_Troop;
        public TMP_Text Text_Emperor;
        public TMP_Text Text_SpacingGuild;
        public TMP_Text Text_BeneGesserit;
        public TMP_Text Text_Fremen;
        public TMP_Text Text_Persuasion;
        public GameObject AgentSeat_1;
        public GameObject AgentSeat_2;
        public GameObject AgentSeat_3;
        public GameObject AgentSeat_4;
        public GameObject Panel_Attack;
        public TMP_Text Text_Attack;

        private List<GameObject> Object_Agents;

        [Header("Player #1")]
        public GameObject Panel_Player1;
        public TMP_Text Text_Player1_VictoryPoint;
        public TMP_Text Text_Player1_Lunar;
        public TMP_Text Text_Player1_Marsion;
        public TMP_Text Text_Player1_Water;
        public TMP_Text Text_Player1_Troop;
        public TMP_Text Text_Player1_Emperor;
        public TMP_Text Text_Player1_SpacingGuild;
        public TMP_Text Text_Player1_BeneGesserit;
        public TMP_Text Text_Player1_Fremen;
        public TMP_Text Text_Player1_Persuasion;

        [Header("Player #2")]
        public GameObject Panel_Player2;
        public TMP_Text Text_Player2_VictoryPoint;
        public TMP_Text Text_Player2_Lunar;
        public TMP_Text Text_Player2_Marsion;
        public TMP_Text Text_Player2_Water;
        public TMP_Text Text_Player2_Troop;
        public TMP_Text Text_Player2_Emperor;
        public TMP_Text Text_Player2_SpacingGuild;
        public TMP_Text Text_Player2_BeneGesserit;
        public TMP_Text Text_Player2_Fremen;
        public TMP_Text Text_Player2_Persuasion;

        [Header("Player #3")]
        public GameObject Panel_Player3;
        public TMP_Text Text_Player3_VictoryPoint;
        public TMP_Text Text_Player3_Lunar;
        public TMP_Text Text_Player3_Marsion;
        public TMP_Text Text_Player3_Water;
        public TMP_Text Text_Player3_Troop;
        public TMP_Text Text_Player3_Emperor;
        public TMP_Text Text_Player3_SpacingGuild;
        public TMP_Text Text_Player3_BeneGesserit;
        public TMP_Text Text_Player3_Fremen;
        public TMP_Text Text_Player3_Persuasion;

        public override void Activate()
        {
            // this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.UpdateData), Container);
            // Object_Agents = new();

            LocalPlayerView.Activate();
        }

        public override void Deactivate()
        {
            LocalPlayerView.Deactivate();
            // this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.UpdateData), Container);
        }

        private void OnUpdateData(object sender, object args)
        {
            Data = new();

            int nonLocalIndex = 0;

            foreach (var player in GetComponent<MatchView>().Data.Players)
            {
                Data.Add(player);

                // 단 이 클라이언트에 해당하는 Player일 경우, 별도의 UI를 업데이트
                if (player.ClientID == NetworkManager.Singleton.LocalClientId)
                {
                    ClientPlayer = player;

                    Text_VictoryPoint.text = ClientPlayer.VictoryPoint.ToString();
                    Text_Lunar.text = ClientPlayer.Lunar.ToString();
                    Text_Marsion.text = ClientPlayer.Marsion.ToString();
                    Text_Water.text = ClientPlayer.Water.ToString();
                    Text_Troop.text = ClientPlayer.Troop.ToString();
                    Text_Persuasion.text = (ClientPlayer.Persuasion + ClientPlayer.BasePersuasion).ToString();
                    Text_Emperor.text = ClientPlayer.EmperorInfluence.ToString();
                    Text_SpacingGuild.text = ClientPlayer.SpacingGuildInfluence.ToString();
                    Text_BeneGesserit.text = ClientPlayer.BeneGesseritInfluence.ToString();
                    Text_Fremen.text = ClientPlayer.FremenInfluence.ToString();

                    if (ClientPlayer.Attack > 0)
                    {
                        Text_Attack.text = ClientPlayer.Attack.ToString();
                        Panel_Attack.SetActive(true);
                    }
                    else
                    {
                        Panel_Attack.SetActive(false);
                        Text_Attack.text = "-1";
                    }


                    foreach (var agent in Object_Agents)
                    {
                        Destroy(agent);
                    }

                    GameObject[] agentSeats = { AgentSeat_1, AgentSeat_2, AgentSeat_3, AgentSeat_4 };
                    uint availableAgentCount = ClientPlayer.TotalAgentCount - ClientPlayer.UsedAgentCount;

                    for (uint i = availableAgentCount; i > 0; i--)
                    {
                        var agentView = Instantiate(Prefab_AgentView, agentSeats[i - 1].transform);
                        Object_Agents.Add(agentView);
                    }
                }
                else
                {
                    switch (nonLocalIndex)
                    {
                        case 0:
                            Panel_Player1.SetActive(true);
                            Text_Player1_VictoryPoint.text = player.VictoryPoint.ToString();
                            Text_Player1_Lunar.text = player.Lunar.ToString();
                            Text_Player1_Marsion.text = player.Marsion.ToString();
                            Text_Player1_Water.text = player.Water.ToString();
                            Text_Player1_Troop.text = player.Troop.ToString();
                            Text_Player1_Emperor.text = player.EmperorInfluence.ToString();
                            Text_Player1_SpacingGuild.text = player.SpacingGuildInfluence.ToString();
                            Text_Player1_BeneGesserit.text = player.BeneGesseritInfluence.ToString();
                            Text_Player1_Fremen.text = player.FremenInfluence.ToString();
                            Text_Player1_Persuasion.text = (player.Persuasion + player.BasePersuasion).ToString();
                            break;
                        case 1:
                            Panel_Player2.SetActive(true);
                            Text_Player2_VictoryPoint.text = player.VictoryPoint.ToString();
                            Text_Player2_Lunar.text = player.Lunar.ToString();
                            Text_Player2_Marsion.text = player.Marsion.ToString();
                            Text_Player2_Water.text = player.Water.ToString();
                            Text_Player2_Troop.text = player.Troop.ToString();
                            Text_Player2_Emperor.text = player.EmperorInfluence.ToString();
                            Text_Player2_SpacingGuild.text = player.SpacingGuildInfluence.ToString();
                            Text_Player2_BeneGesserit.text = player.BeneGesseritInfluence.ToString();
                            Text_Player2_Fremen.text = player.FremenInfluence.ToString();
                            Text_Player2_Persuasion.text = (player.Persuasion + player.BasePersuasion).ToString();
                            break;
                        case 2:
                            Panel_Player3.SetActive(true);
                            Text_Player3_VictoryPoint.text = player.VictoryPoint.ToString();
                            Text_Player3_Lunar.text = player.Lunar.ToString();
                            Text_Player3_Marsion.text = player.Marsion.ToString();
                            Text_Player3_Water.text = player.Water.ToString();
                            Text_Player3_Troop.text = player.Troop.ToString();
                            Text_Player3_Emperor.text = player.EmperorInfluence.ToString();
                            Text_Player3_SpacingGuild.text = player.SpacingGuildInfluence.ToString();
                            Text_Player3_BeneGesserit.text = player.BeneGesseritInfluence.ToString();
                            Text_Player3_Fremen.text = player.FremenInfluence.ToString();
                            Text_Player3_Persuasion.text = (player.Persuasion + player.BasePersuasion).ToString();
                            break;
                        default:
                            break;
                    }

                    nonLocalIndex++;
                }
            }
        }
    }
}