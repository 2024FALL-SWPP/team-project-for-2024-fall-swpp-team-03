namespace SWPPT3.Main.Prop
{
    public abstract class StatefulProp : PropBase
    {
        protected const int On = 1;
        protected const int Off = 0;
        protected const int Full = 100;

        public int StateChangeAmount;

        public int State { get; set; } = 0;
        public abstract void StateChangeEvent();

        public virtual void Activate()
        {
            State = On;
        }

        public virtual void Deactivate()
        {
            State = Off;
        }
    }

}
