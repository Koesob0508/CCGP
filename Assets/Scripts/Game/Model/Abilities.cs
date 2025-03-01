using System.Collections.Generic;
using CCGP.AspectContainer;

namespace CCGP.Server
{
    public class Abilities : IAspect
    {
        public IContainer Container { get; set; }
        public List<Ability> AbilityList = new List<Ability>();

        public void AddAbility(Ability ability)
        {
            // ability.Container = Container;
            AbilityList.Add(ability);
        }
    }
}