using SWPPT3.Main.Utility;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class Vent : StateDst
    {
        [SerializeField]
        private Animator animator;

        [SerializeField] private PropScript _propscript;


        protected override void OnSourceStateChanged(StateSource src, bool state)
        {
            state = !state;
            State = state;
            animator.SetBool("IsClosed",state);
        }
    }
}
