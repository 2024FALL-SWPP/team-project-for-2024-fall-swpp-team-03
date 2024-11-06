using SWPPT3.Main.Player;

namespace SWPPT3.Main.Prop
{
    public class ItemBox : StatelessPropBase
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
