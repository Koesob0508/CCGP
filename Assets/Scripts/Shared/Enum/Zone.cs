using System;

namespace CCGP.Shared
{
    [Flags]
    public enum Zone
    {
        None = 0,
        Leader = 1 << 0,
        Deck = 1 << 1,
        Hand = 1 << 2,
        Graveyard = 1 << 3,
        Agent = 1 << 4,
        Reveal = 1 << 5,
        ImperiumDeck = 1 << 6,
        ImperiumRow = 1 << 7,
        Play = Agent | Reveal
    }

    public static class ZoneExtensions
    {
        public static bool Contains(this Zone source, Zone target)
        {
            return (source & target) == target;
        }
    }
}