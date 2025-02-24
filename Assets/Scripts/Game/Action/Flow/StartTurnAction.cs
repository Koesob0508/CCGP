namespace CCGP.Server
{
    public class StartTurnAction : GameAction
    {
        public Match Match;
        public int TargetPlayerIndex;

        public StartTurnAction(int targetPlayerIndex)
        {
            TargetPlayerIndex = targetPlayerIndex;
        }
    }
}