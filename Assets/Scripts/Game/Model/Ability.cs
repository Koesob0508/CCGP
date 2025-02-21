using CCGP.AspectContainer;

namespace CCGP.Server
{
    public class Ability
    {
        public Player Player { get; set; }
        public string ActionName { get; set; }
        public object UserInfo { get; set; }
    }
}