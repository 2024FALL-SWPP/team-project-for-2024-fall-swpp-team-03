#region

using SWPPT3.Main.Prop;
using UnityEngine.UIElements;

#endregion

namespace SWPPT3.Main.PlayerLogic.State
{
    public class RubberState : PlayerState
    {
        public override void InteractWithProp(Player player, PropBase obstacle)
        {
            if (obstacle is WoodBox box || obstacle is MetalBox metalBox)
            {
                player.CollisionSound();
            }
            else
            {
                base.InteractWithProp(player, obstacle);
            }
        }
    }
}
