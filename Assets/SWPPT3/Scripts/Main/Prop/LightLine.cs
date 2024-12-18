#region

using UnityEngine;

#endregion

namespace SWPPT3.Main.Prop
{
    public class LightLine : StateDst
    {
        [SerializeField] private Animator animator;

        protected override void OnSourceStateChanged(StateSource src, bool state)
        {
            if (src.State == On)
            {
                animator.SetBool("IsOn", true);
            }
            else if (src.State == Off)
            {
                animator.SetBool("IsOn", false);
            }
        }
    }
}
