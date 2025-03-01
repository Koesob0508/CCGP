using System.Collections.Generic;
using System.Linq;
using CCGP.Shared;
using UnityEngine;

namespace CCGP.Server
{
    public class RoundFactory
    {
        private static Dictionary<string, Dictionary<string, object>> _roundRewards = null;
        private static Dictionary<string, Dictionary<string, object>> RoundRewards
        {
            get
            {
                if (_roundRewards == null)
                {
                    _roundRewards = LoadRoundRewards();
                }
                return _roundRewards;
            }
        }

        public static Round Create()
        {
            var round = new Round();
            round.Count = 0;
            round.RewardDeck = CreateRoundRewardDeck();
            return round;
        }

        private static Dictionary<string, Dictionary<string, object>> LoadRoundRewards()
        {
            var file = Resources.Load<TextAsset>("RoundReward");

            if (file == null)
            {
                LogUtility.LogError("", colorName: ColorCodes.Red);
                return null;
            }

            var dict = MiniJSON.Json.Deserialize(file.text) as Dictionary<string, object>;

            Resources.UnloadAsset(file);

            var rewards = (List<object>)dict["RoundRewards"];
            var result = new Dictionary<string, Dictionary<string, object>>();

            foreach (var reward in rewards)
            {
                var rewardData = (Dictionary<string, object>)reward;
                var id = (string)rewardData["ID"];
                result.Add(id, rewardData);
            }

            return result;
        }

        private static Stack<RoundReward> CreateRoundRewardDeck()
        {
            var result = new Stack<RoundReward>();

            RoundReward dummyReward = null;
            var phase1Reward = new List<RoundReward>();
            var phase2Reward = new List<RoundReward>();
            var phase3Reward = new List<RoundReward>();

            // 페이즈 별로 나눠야한다.
            // RoundReward 만들기
            foreach (var data in RoundRewards.Values)
            {
                var reward = new RoundReward();
                reward.Load(data);

                switch (reward.Phase)
                {
                    case 0:
                        dummyReward = reward;
                        break;
                    case 1:
                        phase1Reward.Add(reward);
                        break;
                    case 2:
                        phase2Reward.Add(reward);
                        break;
                    case 3:
                        phase3Reward.Add(reward);
                        break;
                    default:
                        LogUtility.LogWarning($"[RoundRewardFactory] 잘못된 페이즈 입력됨 : {reward.Phase}", colorName: ColorCodes.Red);
                        break;
                }
            }

            // 그리고 각 페이즈로부터 2, 5, 3 이렇게 뽑을까
            // 랜덤 객체 생성
            System.Random rng = new System.Random();

            // Phase1: 2개, Phase2: 5개, Phase3: 3개를 랜덤하게 선택
            var selectedPhase1 = phase1Reward.OrderBy(x => rng.Next()).Take(2).ToList();
            var selectedPhase2 = phase2Reward.OrderBy(x => rng.Next()).Take(5).ToList();
            var selectedPhase3 = phase3Reward.OrderBy(x => rng.Next()).Take(3).ToList();

            foreach (var reward in phase3Reward)
            {
                result.Push(reward);
            }

            foreach (var reward in phase2Reward)
            {
                result.Push(reward);
            }

            foreach (var reward in phase1Reward)
            {
                result.Push(reward);
            }

            result.Push(dummyReward);

            return result;
        }

        private static RoundReward CreateRoundReward(string id)
        {
            var roundReward = new RoundReward();
            var rewardData = RoundRewards[id];

            roundReward.Load(rewardData);

            return roundReward;
        }
    }
}