using CCGP.Shared;
using System.Collections.Generic;

namespace CCGP.AspectContainer
{
    public interface IEntity
    {
        ICollection<IAspect> Aspects { get; }
        T AddAspect<T>(string key = null) where T : IAspect, new();
        T AddAspect<T>(T aspect, string key = null) where T : IAspect;
        bool TryGetAspect<T>(string key, out T aspect) where T : IAspect;
        bool TryGetAspect<T>(out T aspect) where T : IAspect;
    }

    public class Entity : IEntity
    {
        private readonly Dictionary<string, IAspect> aspects = new();
        public ICollection<IAspect> Aspects => aspects.Values;

        public T AddAspect<T>(string key = null) where T : IAspect, new()
        {
            return AddAspect(new T(), key);
        }
        public T AddAspect<T>(T aspect, string key = null) where T : IAspect
        {
            key = key ?? typeof(T).Name;
            aspects.Add(key, aspect);
            aspect.Entity = this;
            return aspect;
        }
        public bool TryGetAspect<T>(out T aspect) where T : IAspect
        {
            return TryGetAspect("", out aspect);
        }
        public bool TryGetAspect<T>(string key, out T aspect) where T : IAspect
        {
            key = string.IsNullOrEmpty(key) ? typeof(T).Name : key;
            if(aspects.ContainsKey(key))
            {
                aspect = (T)aspects[key];
                return true;
            }
            else
            {
                Logger.Log<Entity>($"Container didn't has aspect {key}");
                aspect = default;
                return false;
            }
        }
    }
}