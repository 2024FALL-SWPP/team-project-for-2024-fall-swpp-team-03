#region

using SWPPT3.Main.Prop;
using UnityEngine;

#endregion

namespace SWPPT3.Main.ConductorLogic
{
    public class Receptor : Conductor
    {
        [SerializeField]
        private StateSource connectedStateSource;

        public void UpdateState(bool state)
        {
            if (!connectedStateSource.State)
            {
                connectedStateSource.State = state;
            }
            //Debug.Log("Receptor:"+state);
        }
    }
}
