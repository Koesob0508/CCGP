namespace CCGP.Server
{
    public class TurnEndAction : GameAction
    {
        public int TargetPlayerIndex;
        public int NextPlayerIndex;

        public TurnEndAction(int targetPlayerIndex, int nextPlayerIndex)
        {
            TargetPlayerIndex = targetPlayerIndex;
            NextPlayerIndex = nextPlayerIndex;
        }
    }
}