using System;
using CCGP.AspectContainer;

namespace CCGP.Server
{
    public class GenerateCardAction : GameAction, IAbilityLoader
    {
        public string CardID;
        public Card Card;
        public GenerateCardAction() { }
        public void Load(IContainer game, Ability ability)
        {
            Player = ability.Player;
            CardID = Convert.ToString(ability.UserInfo);
        }
    }
}