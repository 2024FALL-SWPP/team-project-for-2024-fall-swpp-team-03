using SWPPT3.Main.Manager;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class MagicCircle : StateDst
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Collider collider;
        [SerializeField] private int circleState;

        public void Awake()
        {
            UpdateCircleState();
        }

        protected override void OnSourceStateChanged(StateSource src, bool state)
        {
            if (maintainState && State) return;
            UpdateCircleState();
        }

        private void UpdateCircleState()
        {
            circleState = 0;

            foreach (var source in stateSources)
            {
                if (source.State)
                {
                    circleState++;
                }
            }

            animator.SetInteger("CircleState", circleState);

            if (circleState == stateSources.Count)
            {
                State = true;
                ActivateMagicCircle();
            }
            else
            {
                State = false;
                DeactivateMagicCircle();
            }
        }

        private void ActivateMagicCircle()
        {
            collider.enabled = true;
        }

        private void DeactivateMagicCircle()
        {
            collider.enabled = false;
        }
    }
}
