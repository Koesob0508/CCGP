using System.Collections.Generic;
using CCGP.AspectContainer;
using CCGP.Shared;
using UnityEngine;

namespace CCGP.Server
{
	public class CardSystem : Aspect, IActivatable
	{
		public void Activate()
		{
			this.AddObserver(OnPrepareGenerateCard, Global.PrepareNotification<GenerateCardAction>(), Container);
		}

		public void Deactivate()
		{
			this.RemoveObserver(OnPrepareGenerateCard, Global.PrepareNotification<GenerateCardAction>(), Container);
		}

		public void ChangeZone(Card card, Zone zone, Player toPlayer = null)
		{
			var fromPlayer = Container.GetMatch().Players[card.OwnerIndex];
			toPlayer = toPlayer ?? fromPlayer;

			if (card.Zone != Zone.None)
			{
				fromPlayer[card.Zone].Remove(card);
			}

			toPlayer[zone].Add(card);

			card.Zone = zone;
			card.OwnerIndex = toPlayer.Index;
		}

		private void OnPrepareGenerateCard(object sender, object args)
		{
			var action = args as GenerateCardAction;
			var card = CardFactory.CreateCard(action.CardID, action.Player.Index);
			action.Card = card;
		}
	}

	public static class CardSystemExtensions
	{
		public static void SetZone(this Card card, Zone zone)
		{
			card.Zone = zone;
		}
	}
}