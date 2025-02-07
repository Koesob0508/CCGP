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
    }
}