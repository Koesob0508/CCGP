using CCGP.AspectContainer;
using System;
using System.Collections.Generic;

namespace CCGP.Server
{
    public class Tile : Container, IAspect
    {
        private const int defaultIndex = -1;
        public IContainer Container { get; set; }
        public Board Board => Container as Board;
        public int AgentIndex { get; set; } = defaultIndex;

        public string ID;
        public string Name;
        public Space Space;
        
        public virtual void Load(Dictionary<string, object> data)
        {
            ID = (string)data["ID"];
            Name = (string)data["Name"];
            var strSpace = (string)data["Space"];
            Enum.TryParse<Space>(strSpace.ToString(), out var result);
            Space = result;
        }
    }
}