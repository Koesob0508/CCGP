namespace CCGP.Server
{
    public static class ServerFactory
    {
        public static Server Create()
        {
            var server = new Server();
            var game = GameFactory.Create();
            server.AddAspect(game);

            return server;
        }
    }
}