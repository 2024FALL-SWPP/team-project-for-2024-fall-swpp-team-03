using SWPPT3.Main.Prop;

namespace SWPPT3.Main.PlayerLogic.State
{
    public enum States
    {
        Slime = 0,
        Metal,
        Rubber,
    }

    public abstract class PlayerState
    {
        public virtual void InteractWithProp(PropBase obstacle)
        {

        }
    }

}
