using CCGP.AspectContainer;
using CCGP.Shared;

namespace CCGP.Server
{
    public class Cost : Aspect
    {
        public CostType Type;
        public uint Amount;
    }
}