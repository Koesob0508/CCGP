using CCGP.AspectContainer;
using System.Collections.Generic;

namespace CCGP.Server
{
    public class Game : Container, IAspect
    {
        public IContainer Container { get; set; }
        public Server Server => Container as Server;

        public List<PlayerInfo> PlayerInfos { get; set; }
    }
}