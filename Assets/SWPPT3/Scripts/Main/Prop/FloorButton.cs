using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class FloorButton : StateSource
    {
        [SerializeField] private Animator animator;

        private void OnCollisionEnter(Collision other)
        {
            State = On;
            animator.SetBool("IsPressed", true);
        }

        private void OnCollisionExit(Collision other)
        {
            State = Off;
            animator.SetBool("IsPressed", false);
        }
    }
}
