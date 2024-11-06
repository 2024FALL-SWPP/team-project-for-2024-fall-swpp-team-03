using SWPPT3.Main.Player;
using SWPPT3.Main.Player.State;

namespace SWPPT3.Main.Prop
{
    public class ItemBox : StatelessProp
    {
        private States _state;

        public ItemBox(States state)
        {
            _state = state;
        }

        public override void InteractWithPlayer()
        {
            // Destroy(gameObject);
        }
    }
}
