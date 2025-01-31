using UnityEngine;

namespace CCGP.Shared
{
    /// <summary>
    /// Interface for logging, designed to support mocking in tests.
    /// </summary>
    public interface ILogger
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
    }

    /// <summary>
    /// Default implementation of ILogger using Unity's Debug class.
    /// </summary>
    public class DefaultLogger : ILogger
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }

        public void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        public void LogError(string message)
        {
            Debug.LogError(message);
        }
    }
}