using System;
using System.Collections.Generic;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class FloorButton : StateSource
    {
        private void OnCollisionEnter(Collision other)
        {
            State = On;
        }

        private void OnCollisionExit(Collision other)
        {
            State = Off;
        }
    }
}
