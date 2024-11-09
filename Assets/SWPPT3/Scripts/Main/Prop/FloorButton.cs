using System;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class FloorButton : StateSource
    {
        public StatefulProp targetProp;
        public ElectricWire electricWire;

        public override void StateChangeEvent()
        {
            if (this.State == (int)StateLevel.On)
            {
                ActivateState(targetProp, StateLevel.On);
                ActivateState(electricWire, StateLevel.On);
            }
            else
            {
                ActivateState(targetProp, StateLevel.Off);
                ActivateState(electricWire, StateLevel.Off);
            }
        }
        private void OnCollisionEnter(Collision other)
        {
            this.State = (int)StateLevel.On;
            StateChangeEvent();
        }

        private void OnCollisionExit(Collision other)
        {
            this.State = (int)StateLevel.Off;
            StateChangeEvent();
        }
    }
}
