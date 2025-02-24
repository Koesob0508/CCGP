namespace CCGP.Server
{
    public class EndTurnAction : GameAction
    {
        public Match Match;
        public int TargetPlayerIndex;
        public int NextPlayerIndex;

        public EndTurnAction(int targetPlayerIndex, int nextPlayerIndex)
        {
            TargetPlayerIndex = targetPlayerIndex;
            NextPlayerIndex = nextPlayerIndex;
        }
    }
}