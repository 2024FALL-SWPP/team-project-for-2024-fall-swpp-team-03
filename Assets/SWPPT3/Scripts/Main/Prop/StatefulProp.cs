using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public abstract class StatefulProp : PropBase
    {
        public const bool On = true;
        public const bool Off = false;
        public virtual bool State { get; set; }
        protected bool PropState;
    }

}
