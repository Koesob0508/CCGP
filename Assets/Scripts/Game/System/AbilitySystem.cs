using System;
using CCGP.AspectContainer;
using CCGP.Shared;

namespace CCGP.Server
{
    public class AbilitySystem : Aspect, IActivatable
    {
        public void Activate()
        {
            this.AddObserver(OnPerformPlayCard, Global.PerformNotification<PlayCardAction>(), Container);
            this.AddObserver(OnPerformAbility, Global.PerformNotification<AbilityAction>(), Container);
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnPerformPlayCard, Global.PerformNotification<PlayCardAction>(), Container);
            this.RemoveObserver(OnPerformAbility, Global.PerformNotification<AbilityAction>(), Container);
        }

        private void OnPerformPlayCard(object sender, object args)
        {
            var action = args as PlayCardAction;

            if (action.Card.TryGetAspect(out Target target))
            {
                if (target.Selected.TryGetAspect(out Abilities abilities))
                {
                    foreach (var ability in abilities.AbilityList)
                    {
                        var abilityAction = new AbilityAction(action.Player, ability);
                        Container.AddReaction(abilityAction);
                    }
                }
            }
        }

        private void OnPerformAbility(object sender, object args)
        {
            var action = args as AbilityAction;
            var type = Type.GetType($"CCGP.Server.{action.Ability.ActionName}");
            var instance = Activator.CreateInstance(type) as GameAction;
            var loader = instance as IAbilityLoader;
            if (loader != null)
            {
                loader.Load(Container, action.Ability);
            }
            Container.AddReaction(instance);
        }
    }
}