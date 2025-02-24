namespace CCGP.Server
{
    public class TryOpenCardsAction : GameAction
    {
        public Player Player;
        public TryOpenCardsAction(Player player)
        {
            Player = player;
        }
    }
}