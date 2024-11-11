using System;
using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.PlayerLogic.State;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class ItemBox : StatelessProp
    {
        public PlayerStates ItemState;

        public override void InteractWithPlayer()
        {
            Destroy(gameObject);
        }
    }
}
