using System;

namespace SWPPT3.Main.Prop
{
    public abstract class StateSource: StatefulProp
    {
        public event Action<StateSource,bool> OnStateChanged;
        public override bool State
        {
            get => PropState;
            set
            {
                if (PropState != value)
                {
                    PropState = value;
                    OnStateChanged?.Invoke(this,PropState);
                }
            }
        }
    }
}
