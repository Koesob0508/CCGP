using CCGP.AspectContainer;

namespace CCGP.Server
{
    public enum ResourceType
    {
        Marsion,
        Lunar,
        Water
    }

    public class Cost : Aspect
    {
        public ResourceType Type;
        public uint Amount;
    }
}