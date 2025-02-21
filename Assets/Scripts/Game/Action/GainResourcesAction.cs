using System;
using System.Collections.Generic;
using CCGP.AspectContainer;
using CCGP.Shared;

namespace CCGP.Server
{
    public class GainResourcesAction : GameAction, IAbilityLoader
    {
        public ResourceType Type;
        public uint Amount;

        // IAbilityLoader의 기능이 올바르게 동작하기 위해서는 기본 생성자가 필요함
        public GainResourcesAction() { }
        public GainResourcesAction(Player player, ResourceType type, uint amount)
        {
            Player = player;
            Type = type;
            Amount = amount;
        }

        public void Load(IContainer game, Ability ability)
        {
            Player = ability.Player;
            var data = (Dictionary<string, object>)ability.UserInfo;
            Enum.TryParse<ResourceType>(data["ResourceType"].ToString(), out var type);
            Type = type;
            Amount = Convert.ToUInt16(data["Amount"]);
        }
    }
}