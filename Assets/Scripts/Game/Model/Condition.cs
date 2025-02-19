using CCGP.AspectContainer;

namespace CCGP.Server
{
    public enum FactionType
    {
        Emperor,
        SpacingGuild,
        BeneGesserit,
        Fremen
    }
    public class Condition : Aspect
    {
        public FactionType Type;
        public uint Amount;
    }
}