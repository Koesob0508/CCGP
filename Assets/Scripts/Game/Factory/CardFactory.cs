using System.Collections.Generic;
using CCGP.Shared;
using UnityEngine;

namespace CCGP.Server
{
    public static class CardFactory
    {
        private static Dictionary<string, Dictionary<string, object>> _cards = null;
        private static Dictionary<string, Dictionary<string, object>> Cards
        {
            get
            {
                if (_cards == null)
                {
                    _cards = LoadInitialCards();
                }
                return _cards;
            }
        }

        private static Dictionary<string, Dictionary<string, object>> LoadInitialCards()
        {
            var file = Resources.Load<TextAsset>("CardList");

            if (file == null)
            {
                LogUtility.LogError<Card>("CardList.json을 찾을 수 없습니다!");
                return null;
            }

            var dict = MiniJSON.Json.Deserialize(file.text) as Dictionary<string, object>;

            Resources.UnloadAsset(file);

            var cardList = (List<object>)dict["Cards"];
            var result = new Dictionary<string, Dictionary<string, object>>();
            foreach (object entry in cardList)
            {
                var cardData = (Dictionary<string, object>)entry;
                var id = (string)cardData["ID"];
                result.Add(id, cardData);
            }

            return result;
        }

        public static List<Card> CreateDeck(string fileName, int ownerIndex)
        {
            var file = Resources.Load<TextAsset>(fileName);
            var contents = MiniJSON.Json.Deserialize(file.text) as Dictionary<string, object>;
            Resources.UnloadAsset(file);

            var array = (List<object>)contents["Deck"];
            var result = new List<Card>();
            foreach (object item in array)
            {
                var id = (string)item;
                var card = CreateCard(id, ownerIndex);
                card.SetZone(Zone.Deck);
                result.Add(card);
            }

            return result;
        }

        public static Card CreateCard(string id, int ownerIndex)
        {
            var cardData = Cards[id];
            Card card = CreateCard(cardData, ownerIndex);
            card.AddAspect<Target>();

            return card;
        }

        private static Card CreateCard(Dictionary<string, object> data, int ownerIndex)
        {
            var card = new Card();

            card.Load(data);
            card.OwnerIndex = ownerIndex;

            return card;
        }
    }
}