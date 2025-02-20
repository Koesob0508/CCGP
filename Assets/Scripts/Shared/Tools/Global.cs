using UnityEngine;

namespace CCGP.Shared
{
    public static class Global
    {
        public static int GenerateID<T>()
        {
            return GenerateID(typeof(T));
        }

        public static int GenerateID(System.Type type)
        {
            return Animator.StringToHash(type.Name);
        }

        public static string PrepareNotification<T>()
        {
            return PrepareNotification(typeof(T));
        }

        public static string PrepareNotification(System.Type type)
        {
            return $"{type.Name}.PrepareNotification";
        }

        public static string PerformNotification<T>()
        {
            return PerformNotification(typeof(T));
        }

        public static string PerformNotification(System.Type type)
        {
            return $"{type.Name}.PerformNotification";
        }

        public static string MessageNotification(System.Enum type)
        {
            return $"{type}.MessageNotification";
        }

        public static string ValidateNotification<T>()
        {
            return ValidateNotification(typeof(T));
        }

        public static string ValidateNotification(System.Type type)
        {
            return $"{type.Name}.ValidationNotification";
        }

        public static string CancelNotification<T>()
        {
            return CancelNotification(typeof(T));
        }

        public static string CancelNotification(System.Type type)
        {
            return $"{type.Name}.CancelNotification";
        }

        public static string CancelNotification(System.Enum type)
        {
            return $"{type}.CancelNotification";
        }
    }
}