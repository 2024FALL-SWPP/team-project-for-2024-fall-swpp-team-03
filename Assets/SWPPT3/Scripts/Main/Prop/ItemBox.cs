#region

using SWPPT3.Main.PlayerLogic.State;

#endregion

namespace SWPPT3.Main.Prop
{
    public class ItemBox : StatelessProp
    {
        public PlayerStates ItemState;

        public bool MarkedToBeDestroyed { get; private set; }

        public override void InteractWithPlayer()
        {
            MarkedToBeDestroyed = true;

            Destroy(gameObject);
        }
    }
}
