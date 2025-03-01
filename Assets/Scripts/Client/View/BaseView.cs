using CCGP.AspectContainer;
using UnityEngine;

namespace CCGP.Client
{
    public abstract class BaseView : MonoBehaviour, IAspect, IActivatable
    {
        public IContainer Container { get; set; }

        public virtual void Activate() {}

        public virtual void Deactivate() {}
    }
}