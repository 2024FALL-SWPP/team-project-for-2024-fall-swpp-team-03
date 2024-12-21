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
            if (state)
            {
                Animator receptorAnimation = transform.GetComponent<Animator>();
                receptorAnimation.SetBool("IsOn", true);
                Animator emmiterAnimation = transform.parent.Find("Electric_Wire").GetComponent<Animator>();
                emmiterAnimation.SetBool("IsOn", true);
            }
            //Debug.Log("Receptor:"+state);
        }
    }
}
