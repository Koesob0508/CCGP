using CCGP.Shared;
using System.Collections.Generic;

namespace CCGP.Server
{
    public class Player
    {
        public const int InitialDeck = 10;
        public const int InitialHand = 5;
        public const int InitialAgentCount = 2;
        public const int InitialTurnActionCount = 1;
        public const int InitialWater = 1;

        public const int tempInitialFactionInfluence = 0;

        public ulong ID;
        public readonly int Index;
        public uint AgentCount;
        public uint TurnActionCount;

        public uint Lunar;
        public uint Marsion;
        public uint Water;

        public uint EmperorInfluence;
        public uint SpacingGuildInfluence;
        public uint BeneGesseritInfluence;
        public uint FremenInfluence;

        public PlayerInfo PlayerInfo;

        private List<Card> leader = new List<Card>(1);
        private List<Card> deck = new List<Card>(InitialDeck);
        private List<Card> hand = new List<Card>(InitialHand);
        private List<Card> graveyard = new List<Card>(InitialDeck);
        private List<Card> agent = new List<Card>();
        private List<Card> open = new List<Card>();

        public Player(int index, PlayerInfo playerInfo)
        {
            ID = playerInfo.ClientID;
            Index = index;
            PlayerInfo = playerInfo;

            AgentCount = InitialAgentCount;
            TurnActionCount = InitialTurnActionCount;

            Water = InitialWater;

            EmperorInfluence = tempInitialFactionInfluence;
            SpacingGuildInfluence = tempInitialFactionInfluence;
            BeneGesseritInfluence = tempInitialFactionInfluence;
            FremenInfluence = tempInitialFactionInfluence;
        }

        public List<Card> this[Zone z]
        {
            get
            {
                switch (z)
                {
                    case Zone.Leader:
                        return leader;
                    case Zone.Deck:
                        return deck;
                    case Zone.Hand:
                        return hand;
                    case Zone.Graveyard:
                        return graveyard;
                    case Zone.Agent:
                        return agent;
                    case Zone.Open:
                        return open;
                    default:
                        return null;
                }
            }
        }
    }
}