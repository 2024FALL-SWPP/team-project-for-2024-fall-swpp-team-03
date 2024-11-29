using System;
using System.Collections.Generic;
using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.PlayerLogic.State;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class WoodBox : StatelessProp
    {
        void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Box");
        }
    }
}
