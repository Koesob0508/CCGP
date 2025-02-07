using CCGP.AspectContainer;

namespace CCGP.Shared
{
    public interface IUpdate
    {
        void Update();
    }

    public static class UpdateExtensions
    {
        public static void Update(this IContainer container)
        {
            foreach(IAspect aspect in container.Aspects)
            {
                var item = aspect as IUpdate;
                if (item != null) item.Update();
            }
        }
    }
}