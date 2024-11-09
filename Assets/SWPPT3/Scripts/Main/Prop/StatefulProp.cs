namespace SWPPT3.Main.Prop
{
    public enum StateLevel
    {
        Off = 0,
        On = 100,
        Quarter = 25,
        Half = 50,
        ThreeQuarters = 75,
        Full = 100
    }
    public abstract class StatefulProp : PropBase
    {
        public int State { get; set; } = 0;
        public abstract void StateChangeEvent();
    }

}
