namespace CCGP.Server
{
    public class TurnStartAction : GameAction
    {
        public int TargetPlayerIndex;

        public TurnStartAction(int targetPlayerIndex)
        {
            TargetPlayerIndex = targetPlayerIndex;
        }
    }
}