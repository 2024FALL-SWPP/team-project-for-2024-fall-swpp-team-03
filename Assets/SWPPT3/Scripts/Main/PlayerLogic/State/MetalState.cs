using SWPPT3.Main.Prop;
using UnityEngine;

namespace SWPPT3.Main.PlayerLogic.State
{
    public class MetalState : PlayerState
    {
        public override void InteractWithProp(Player player, PropBase obstacle)
        {
            if (false)
            {
                obstacle.InteractWithPlayer();
            }
            else
            {
                base.InteractWithProp(player, obstacle);
            }
        }
    }
}
