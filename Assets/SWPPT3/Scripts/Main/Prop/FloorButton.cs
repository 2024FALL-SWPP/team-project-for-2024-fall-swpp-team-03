using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class FloorButton : StateSource
    {
        [SerializeField] private Animator animator;

        private void OnTriggerEnter(Collider other)
        {
            State = On;
            animator.SetBool("IsPressed", true);
            Debug.Log("FloorButton"+State);
        }

        private void OnTriggerExit(Collider other)
        {
            State = Off;
            animator.SetBool("IsPressed", false);
            Debug.Log("FloorButton"+State);
        }
    }
}
