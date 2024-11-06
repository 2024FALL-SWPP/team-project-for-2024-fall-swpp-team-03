using SWPPT3.Main.Prop;
using UnityEngine;

namespace SWPPT3.Main.PlayerLogic.State
{
    public class MetalState : PlayerState
    {
        public override void InteractWithProp(PropBase obstacle)
        {
            // player의 변화
            obstacle.InteractWithPlayer();
        }

    }
}
