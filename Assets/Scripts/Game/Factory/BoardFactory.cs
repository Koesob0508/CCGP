namespace CCGP.Server
{
    public static class BoardFactory
    {
        public static Board Create()
        {
            var board = new Board();

            for (int i = 1; i <= 22; i++)
            {
                string id = $"00-{i:D3}";
                var tile = TileFactory.CreateTile(id);
                board.AddAspect(tile, id);
            }

            return board;
        }
    }
}