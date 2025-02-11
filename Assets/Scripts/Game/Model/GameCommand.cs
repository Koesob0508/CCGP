namespace CCGP.Shared
{
    public enum GameCommand : ushort
    {
        None = 0,

        // Client to Server

        // Server to Client
        StartGame = 1000,
        StartRound = 1100,
        StartTurn = 1200,
        DrawCards = 1210,
    }
}