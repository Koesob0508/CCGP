namespace CCGP.Server
{
    public class AbilityAction : GameAction
    {
        public Player Player;
        public Ability Ability;
        public AbilityAction(Player player, Ability ability)
        {
            Player = player;
            ability.Player = player;
            Ability = ability;
        }
    }
}