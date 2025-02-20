namespace CCGP.Server
{
    public static class GameFactory
    {
        public static Game Create()
        {
            Game game = new Game();

            game.AddAspect<DataSystem>();
            game.AddAspect<MatchSystem>();
            game.AddAspect<BoardSystem>();
            game.AddAspect<ActionSystem>();
            game.AddAspect<FlowSystem>();
            game.AddAspect<OpenMarketSystem>();
            game.AddAspect<PlayerSystem>();
            game.AddAspect<CardSystem>();
            game.AddAspect<TargetSystem>();
            game.AddAspect<ConditionSystem>();
            game.AddAspect<CostSystem>();
            game.AddAspect<GameMessageSystem>();

            return game;
        }
    }
}