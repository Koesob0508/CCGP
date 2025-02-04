using CCGP.AspectContainer;

namespace CCGP
{
    public interface ISleep
    {
        void Sleep();
    }

    public static class SleepExtensions
    {
        public static void Sleep(this IContainer container)
        {
            foreach (IAspect aspect in container.Aspects)
            {
                var item = aspect as ISleep;
                if (item != null) item.Sleep();
            }
        }
    }
}