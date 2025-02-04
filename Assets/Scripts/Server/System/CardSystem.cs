using CCGP.AspectContainer;
using UnityEngine;

namespace CCGP.Server
{
    public class CardSystem : Aspect
    {
		public void ChangeZone(Card card, Zone zone, Player toPlayer = null)
		{
			var fromPlayer = Container.GetMatch().Players[card.OwnerIndex];
			toPlayer = toPlayer ?? fromPlayer;
			fromPlayer[card.Zone].Remove(card);
			toPlayer[zone].Add(card);
			card.Zone = zone;
			card.OwnerIndex = toPlayer.Index;
		}
	}
}