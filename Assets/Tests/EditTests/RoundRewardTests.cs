using System.Collections.Generic;
using CCGP.Server;
using NUnit.Framework;

namespace CCGP.Tests.Unit
{
    [TestFixture]
    public class RoundRewardTests
    {
        string testJson = @"{
                ""ID"": ""00-001"",
                ""Name"": ""Test Reward"",
                ""Phase"": ""1"",
                ""FirstReward"": {
                    ""Type"": ""RoundReward"",
                    ""Action"": ""GainResourcesAction"",
                    ""Info"": {
                        ""ResourceType"": ""Marsion"",
                        ""Amount"": ""3""
                    }
                },
                ""SecondReward"": {
                    ""Type"": ""RoundReward"",
                    ""Action"": ""GainResourcesAction"",
                    ""Info"": {
                        ""ResourceType"": ""Marsion"",
                        ""Amount"": ""2""
                    }
                },
                ""ThirdReward"": {
                    ""Type"": ""RoundReward"",
                    ""Action"": ""GainResourcesAction"",
                    ""Info"": {
                        ""ResourceType"": ""Marsion"",
                        ""Amount"": ""1""
                    }
                }
            }";

        // Model RoundReward가 Json 데이터로부터 자신을 구성하는 Load가 되는지
        [Test]
        public void LoadTest()
        {
            var data = MiniJSON.Json.Deserialize(testJson) as Dictionary<string, object>;
            var roundReward = new RoundReward();
            roundReward.Load(data);

            Assert.AreEqual("00-001", roundReward.ID, $"ID not equal {roundReward.ID}");
            Assert.AreEqual("Test Reward", roundReward.Name, $"Name not equal {roundReward.Name}");
            Assert.AreEqual(1, roundReward.Phase, $"Tier not equal {roundReward.Phase}");
            Assert.AreEqual(AbilityType.RoundReward, roundReward.FirstReward.Type);
            Assert.AreEqual(AbilityType.RoundReward, roundReward.SecondReward.Type);
            Assert.AreEqual(AbilityType.RoundReward, roundReward.ThirdReward.Type);
            Assert.AreEqual("GainResourcesAction", roundReward.FirstReward.ActionName);
            Assert.AreEqual("GainResourcesAction", roundReward.SecondReward.ActionName);
            Assert.AreEqual("GainResourcesAction", roundReward.ThirdReward.ActionName);
        }

        // 이건 RoundReward Factory가 RoundReward를 생성할 때,
        // JSON 파일을 정확하게 읽어내고, 그 결과 RoundReward를 정확히 생성해낼 수 있는지 테스트
        [Test]
        public void FactoryCreateTest()
        {

        }
    }
}