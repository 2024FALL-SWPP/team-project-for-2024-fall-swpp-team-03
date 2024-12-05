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
            if (circleState == 0)
            {
                collider.enabled = true;
            }
            else
            {
                collider.enabled = false;
            }
            UpdateCircleState();
        }

        protected override void OnSourceStateChanged(StateSource src, bool state)
        {
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
                ActivateMagicCircle();
            }
            else
            {
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
