namespace CCGP.AspectContainer
{
    public interface IAspect
    {
        IEntity Entity { get; set; }
    }
    public class Aspect : IAspect
    {
        public IEntity Entity { get; set; }
    }
}