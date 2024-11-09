using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.PlayerLogic.State;

namespace SWPPT3.Main.Prop
{
    public class ItemBox : StatelessProp
    {
        private States _state;

        public ItemBox(States state)
        {
            _state = state;
        }

        public override void InteractWithPlayer(States state)
        {
            Destroy(gameObject);
        }
    }
}
