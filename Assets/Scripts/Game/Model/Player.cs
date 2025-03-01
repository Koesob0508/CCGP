using CCGP.Shared;
using System.Collections.Generic;

namespace CCGP.Server
{
    public class Player
    {
        public const int InitialDeck = 10;
        public const int InitialHand = 5;
        public const uint InitialMentatCount = 0;
        public const uint InitialBaseAgentCount = 2;
        public const uint InitialTurnActionCount = 1;
        public const uint InitialWater = 2;

        public const uint InitialTroopCount = 3;
        public const uint InitialVictoryPoint = 0;

        public ulong ClientID;
        public readonly int Index;
        public uint BaseAgentCount;
        public uint MentatCount;
        public uint TotalAgentCount => BaseAgentCount + MentatCount;
        public uint UsedAgentCount;
        public uint TurnActionCount;
        public bool IsRevealPhase;
        public bool IsRevealed;

        public uint Lunar;
        public uint Marsion;
        public uint Water;

        public uint EmperorInfluence;
        public uint SpacingGuildInfluence;
        public uint BeneGesseritInfluence;
        public uint FremenInfluence;

        public uint Troop;
        public uint Attack;
        public uint Persuasion;
        public uint BasePersuasion;
        public uint VictoryPoint;

        public PlayerInfo PlayerInfo;

        private List<Card> leader = new List<Card>(1);
        private List<Card> deck = new List<Card>(InitialDeck);
        private List<Card> hand = new List<Card>(InitialHand);
        private List<Card> graveyard = new List<Card>(InitialDeck);
        private List<Card> agent = new List<Card>();
        private List<Card> open = new List<Card>();

        public Player(int index, PlayerInfo playerInfo)
        {
            ClientID = playerInfo.ClientID;
            Index = index;
            PlayerInfo = playerInfo;

            BaseAgentCount = InitialBaseAgentCount;
            MentatCount = InitialMentatCount;

            TurnActionCount = InitialTurnActionCount;
            IsRevealPhase = false;
            IsRevealed = false;

            Water = InitialWater;

            Troop = InitialTroopCount;

            VictoryPoint = InitialVictoryPoint;
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
                    case Zone.Reveal:
                        return open;
                    default:
                        LogUtility.LogError<Player>($"잘못된 Zone입니다: {z}");
                        return null;
                }
            }
        }
    }
}