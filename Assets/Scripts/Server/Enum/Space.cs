using CCGP.Shared;
using System;

namespace CCGP.Server
{
    [Flags]
    public enum Space
    {
        None = 0,
        Yellow = 1 << 0,
        Blue = 1 << 1,
        Green = 1 << 2,
        Emperor = 1 << 3,
        SpacingGuild = 1 << 4,
        BeneGesserit = 1 << 5,
        Fremen = 1 << 6,
        Any = ~0
    }

    public static class SpaceExtensions
    {
        public static bool Contains(this Space source, Space target)
        {
            if (target == Space.None) return false;
            return (source & target) == target;
        }
    }
}