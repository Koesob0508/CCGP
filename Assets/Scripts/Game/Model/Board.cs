using CCGP.AspectContainer;
using System.Collections.Generic;
using System.Linq;

namespace CCGP.Server
{
    public class Board : Container
    {
        public List<Tile> Tiles
        {
            get
            {
                return Aspects.OfType<Tile>().ToList();
            }
        }

        public Tile GetTile(Tile target)
        {
            Tile result = null;

            foreach (var tile in Tiles)
            {
                if (tile.Name == target.Name)
                {
                    result = tile;
                }
            }

            return result;
        }
    }
}