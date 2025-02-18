namespace CCGP.Shared
{
    public enum GameCommand : ushort
    {
        None = 0,
        // Server to Client
        StartGame = 1000,
        StartRound = 1100,
        StartTurn = 1200,
        EndTurn = 1300,
        EndRound = 1400,
        EndGame = 1500,

        DrawCards = 1210,
        ShowAvailableTiles = 1220,
        PlayCard = 1231,
        CancelPlayCard = 1232,

        // Client to Server
        TryPlayCard = 5000,
        TrySelectTile = 5100,
        TryEndTurn = 5200,
    }
}