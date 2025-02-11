using CCGP.AspectContainer;

namespace CCGP.Shared
{
    public interface IUpdate
    {
        bool Enabled { get; }
        void Update();
    }

    public static class UpdateExtensions
    {
        public static void Update(this IContainer container)
        {
            foreach(IAspect aspect in container.Aspects)
            {
                if (aspect is IUpdate item && item.Enabled) item.Update();
            }
        }
    }
}