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
        Guild = 1 << 4,
        BeneGesserit = 1 << 5,
        Fremen = 1 << 6
    }

    public static class SpaceExtensions
    {
        public static bool Contains(this Space source, Space target)
        {
            return (source & target) == target;
        }
    }
}