using CCGP.AspectContainer;

namespace CCGP
{
    public interface IActivate
    {
        void Activate();
    }

    public static class ActivateExtensions
    {
        public static void Activate(this IContainer container)
        {
            foreach (IAspect aspect in container.Aspects)
            {
                var item = aspect as IActivate;
                if (item != null) item.Activate();
            }
        }
    }
}