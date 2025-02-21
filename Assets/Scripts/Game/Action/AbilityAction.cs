namespace CCGP.Server
{
    public class AbilityAction : GameAction
    {
        public Ability Ability;
        public AbilityAction(Player player, Ability ability)
        {
            Player = player;
            ability.Player = player;
            Ability = ability;
        }
    }
}