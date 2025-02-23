namespace CCGP.Shared
{
    public enum GameCommand : ushort
    {
        None = 0,

        // Server to Client

        UpdateData = 100,

        // Phase Command
        StartGame = 1000,
        StartRound = 2000,
        StartTurn = 3000,
        EndTurn = 4000,
        EndRound = 5000,
        EndGame = 6000,

        // Not Phase, But Change
        CancelTryPlayCard = 3231,
        PlayCard = 3232,
        CancelPlayCard = 3233,
        OpenCards = 3234,

        // Not Phase, But Change, It's Game Action
        DrawCards = 3310,
        GainResources = 3320,
        GenerateCard = 3330,
        ShowAvailableTiles = 3340,
        RoundStartDraw = 3350,


        // Client to Server
        TryPlayCard = 7000,
        TryCancelPlayCard = 7010,
        TrySelectTile = 7100,
        TryEndTurn = 7200,
        TryOpenCards = 7300,
    }
}