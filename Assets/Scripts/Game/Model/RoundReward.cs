using System;
using System.Collections.Generic;

namespace CCGP.Server
{
    // RoundReward 말고 별도의 Round Model이 필요함
    public class RoundReward
    {
        public string ID;
        public string Name;
        public uint Phase;
        public Ability FirstReward;
        public Ability SecondReward;
        public Ability ThirdReward;

        public virtual void Load(Dictionary<string, object> data)
        {
            ID = (string)data["ID"];
            Name = (string)data["Name"];
            Phase = Convert.ToUInt16(data["Phase"]);

            var aData1 = data["FirstReward"] as Dictionary<string, object>;
            var aData2 = data["SecondReward"] as Dictionary<string, object>;
            var aData3 = data["ThirdReward"] as Dictionary<string, object>;

            FirstReward = new();
            FirstReward.Load(aData1);

            SecondReward = new();
            SecondReward.Load(aData2);

            ThirdReward = new();
            ThirdReward.Load(aData3);
        }
    }
}