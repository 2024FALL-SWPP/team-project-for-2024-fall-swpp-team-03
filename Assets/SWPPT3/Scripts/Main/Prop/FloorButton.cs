using System;
using System.Collections.Generic;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class FloorButton : StateSource
    {
        public List<StatefulProp> targetProps = new List<StatefulProp>();

        public override void ActivateProp(StatefulProp prop, int state)
        {
            if (state == On) prop.Activate();
            else prop.Deactivate();
            prop.StateChangeEvent();
        }

        public override void StateChangeEvent()
        {
            foreach (StatefulProp prop in targetProps)
            {
                ActivateProp(prop, this.State);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            this.State = On;
            StateChangeEvent();
        }

        private void OnCollisionExit(Collision other)
        {
            this.State = Off;
            StateChangeEvent();
        }
    }
}
