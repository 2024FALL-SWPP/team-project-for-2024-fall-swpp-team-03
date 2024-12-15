using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class ElectricWire : StateDst
    {
        [SerializeField] private Animator animator;

        protected override void OnSourceStateChanged(StateSource src, bool state)
        {
            if (animator != null)
            {
                if (this.State == On)
                {
                    animator.SetBool("IsOn", true);
                }
                else if (this.State == Off)
                {
                    animator.SetBool("IsOn", false);
                }
            }
        }
    }
}
