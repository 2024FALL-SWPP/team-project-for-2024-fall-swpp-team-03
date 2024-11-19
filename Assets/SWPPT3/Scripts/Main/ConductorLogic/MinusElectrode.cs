using UnityEngine;
using SWPPT3.Main.Prop;

namespace SWPPT3.Main.ConductorLogic
{
    public class MinusElectrode : Conductor
    {
        [SerializeField]
        private StateSource connectedStateSource;

        public void UpdatePower(bool newFlowState)
        {
            if (CurrentFlow != newFlowState)
            {
                CurrentFlow = newFlowState;

                if (connectedStateSource != null)
                {
                    connectedStateSource.State = CurrentFlow;
                }

            }
        }
    }
}
