using System;
using System.Collections.Generic;
using CCGP.AspectContainer;

namespace CCGP.Server
{
    public class Ability
    {
        public AbilityType Type { get; set; }
        public Player Player { get; set; }
        public string ActionName { get; set; }
        public object UserInfo { get; set; }

        public virtual void Load(Dictionary<string, object> data)
        {
            Enum.TryParse<AbilityType>((string)data["Type"], out var type);
            Type = type;
            ActionName = (string)data["Action"];
            UserInfo = data["Info"];
        }
    }
}