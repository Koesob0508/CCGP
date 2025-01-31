using CCGP.AspectContainer;
using CCGP.Shared;
using System;

namespace CCGP.Server
{
    public class GameAction
    {
        #region Fields & Properties
        public readonly int ID;
        public Player Player { get; set; }
        public int Priority { get; set; }
        public int OrderOfPlay { get; set; }
        public bool IsCanceled { get; protected set; }
        #endregion

        #region Constructor
        public GameAction()
        {
            ID = Global.GenerateID(GetType());
        }
        #endregion

        #region Public
        public virtual void Cancel()
        {
            IsCanceled = true;
        }
        #endregion
    }
}