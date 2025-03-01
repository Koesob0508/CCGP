using System;
using System.Collections.Generic;
using CCGP.AspectContainer;

namespace CCGP.Server
{
    public class DrawCardsAction : GameAction, IAbilityLoader
    {
        public Player Player;
        public uint Amount;
        public List<Card> Cards;

        // IAbilityLoader의 기능이 올바르게 동작하기 위해서는 기본 생성자가 필요함
        public DrawCardsAction() { }

        public DrawCardsAction(Player player, uint amount)
        {
            Player = player;
            Amount = amount;
        }

        #region IAbility
        public void Load(IContainer game, Ability ability)
        {
            Player = ability.Player;
            Amount = Convert.ToUInt16(ability.UserInfo);
        }
        #endregion
    }
}