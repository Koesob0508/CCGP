using CCGP.AspectContainer;

namespace CCGP.Server
{
    public class CostSystem : Aspect, IActivatable
    {
        public void Activate()
        {
            throw new System.NotImplementedException();
        }

        public void Deactivate()
        {
            throw new System.NotImplementedException();
        }

        private void OnValidateCardPlayAction(object sender, object args)
        {
            var action = sender as CardPlayAction;
            var validator = args as Validator;
        }
    }
}