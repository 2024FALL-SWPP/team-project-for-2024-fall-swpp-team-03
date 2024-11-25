

using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class Door : StateDst
    {
        [SerializeField]
        private Collider collider;

        [SerializeField]
        private Animator animator;
        protected override void OnSourceStateChanged(StateSource src, bool state)
        {
            State = state;
            collider.enabled = state;
            animator.SetBool("DoorActive",state);
        }
    }
}
