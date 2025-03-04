﻿using System;
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
                    _cards = LoadCards();
                }
                return _cards;
            }
        }

        private static Dictionary<string, Dictionary<string, object>> LoadCards()
        {
            var file = Resources.Load<TextAsset>("CardList");

            if (file == null)
            {
                LogUtility.LogError("[CardFactory] CardList.json을 찾을 수 없습니다!", colorName: ColorCodes.Red);
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

        public static List<Card> CreateBaseDeck(string fileName, int ownerIndex)
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

        public static List<Card> CreateImperiumDeck(string fileName)
        {
            var file = Resources.Load<TextAsset>(fileName);
            var contents = MiniJSON.Json.Deserialize(file.text) as Dictionary<string, object>;
            Resources.UnloadAsset(file);

            var array = (List<object>)contents["Deck"];
            var result = new List<Card>();
            foreach (object item in array)
            {
                var id = (string)item;
                var card = CreateCard(id);
                card.SetZone(Zone.ImperiumDeck);
                result.Add(card);
            }

            return result;
        }

        public static Card CreateCard(string id, int ownerIndex)
        {
            var card = CreateCard(id);
            card.OwnerIndex = ownerIndex;

            return card;
        }

        public static Card CreateCard(string id)
        {
            var cardData = Cards[id];
            Card card = CreateCard(cardData);
            card.AddAspect<Target>();
            AddAbilities(card, cardData);

            return card;
        }

        private static Card CreateCard(Dictionary<string, object> data)
        {
            var card = new Card();
            card.GUID = Guid.NewGuid().ToString();
            card.OwnerIndex = -1;
            card.Zone = Zone.None;

            card.Load(data);

            return card;
        }

        private static void AddAbilities(Card card, Dictionary<string, object> data)
        {
            if (data.ContainsKey("Abilities") == false) return;

            var AbilityDatas = (List<object>)data["Abilities"];
            var abilities = card.AddAspect<Abilities>();

            foreach (object entry in AbilityDatas)
            {
                var abilityData = (Dictionary<string, object>)entry;
                var ability = new Ability();
                ability.Load(abilityData);
                abilities.AddAbility(ability);
            }
        }
    }
}