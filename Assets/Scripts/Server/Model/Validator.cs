using CCGP.Shared;

namespace CCGP.Server
{
    public class Validator
    {
        public bool IsValid { get; private set; }
        public Validator()
        {
            IsValid = true;
        }

        public void Invalidate()
        {
            IsValid = false;
        }
    }

    public static class ValidatorExtensions
    {
        public static bool Validate(this object target)
        {
            var validator = new Validator();
            var notificationName = Global.ValidationNotification(target.GetType());
            target.PostNotification(notificationName, validator);
            return validator.IsValid;
        }
    }
}