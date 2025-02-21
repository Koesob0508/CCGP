using CCGP.AspectContainer;

namespace CCGP.Server
{
    public interface IAbilityLoader
    {
        void Load(IContainer game, Ability ability);
    }
}