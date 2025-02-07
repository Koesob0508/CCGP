using System.Collections.Generic;

namespace CCGP.Server
{
    public class Player
    {
        public const int InitialDeck = 10;
        public const int InitialHand = 5;
        public const int InitialAgentCount = 1;

        public ulong ID;
        public readonly int Index;
        public uint AgentCount;

        private PlayerInfo playerInfo;

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
            this.playerInfo = playerInfo;
            AgentCount = InitialAgentCount;
        }

        public List<Card> this[Zone z]
        {
            get
            {
                switch(z)
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