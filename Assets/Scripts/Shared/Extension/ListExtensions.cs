using CCGP.Server;
using System.Collections.Generic;

namespace CCGP.Shared
{
    public static class ListExtensions
    {
        public static bool TryGetCard(this List<Card> cards, string guid, out Card result)
        {
            foreach(var card in cards)
            {
                if (card.GUID == guid)
                {
                    result = card;
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}