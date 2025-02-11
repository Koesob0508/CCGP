using CCGP.AspectContainer;

namespace CCGP
{
    public interface IDeactivate
    {
        void Deactivate();
    }

    public static class DeactivateExtensions
    {
        public static void Deactivate(this IContainer container)
        {
            foreach (IAspect aspect in container.Aspects)
            {
                var item = aspect as IDeactivate;
                if (item != null) item.Deactivate();
            }
        }
    }
}