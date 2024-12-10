using UnityEngine;
using SWPPT3.Main.Prop;

namespace SWPPT3.Main.ConductorLogic
{
    public class Receptor : Conductor
    {
        [SerializeField]
        private StateSource connectedStateSource;

        public void UpdateState(bool state)
        {
            connectedStateSource.State = state;
            //Debug.Log("Receptor:"+state);
        }
    }
}
