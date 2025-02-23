using System.Collections.Generic;
using CCGP.Shared;

namespace CCGP.Server
{
    public static class ImperiumFactory
    {
        public static Imperium Create()
        {
            var imperium = new Imperium();

            // ImperiumDeck을 세팅하고
            imperium.Deck = CardFactory.CreateImperiumDeck("ImperiumDeck");
            // ImperiumRow를 세팅해야함.
            imperium.Row = new();
            // for (int i = 0; i < 5; i++)
            // {
            //     var card = imperium.Deck[0];
            //     imperium.Row.Add(card);
            //     imperium.Deck.RemoveAt(0);
            //     card.SetZone(Zone.ImperiumRow);
            // }

            return imperium;
        }
    }
}