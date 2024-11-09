namespace SWPPT3.Main.Prop
{
    public abstract class StateSource : StatefulProp
    {
        protected virtual void ActivateState(StatefulProp prop, StateLevel level)
        {
            prop.State = (int)level;
            prop.StateChangeEvent();
        }
    }
}
