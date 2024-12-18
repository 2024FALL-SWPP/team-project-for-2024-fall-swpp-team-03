#region

using UnityEngine;

#endregion

namespace SWPPT3.Main.Prop
{
    public class MagicCircle : StateDst
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Collider collider;
        private int satisfiedNumber;
        private int clearStateNumber;

        public void Awake()
        {
            clearStateNumber = stateSources.Count;
            UpdatesatifiedNumber();
        }

        protected override void OnSourceStateChanged(StateSource src, bool state)
        {
            UpdatesatifiedNumber();
        }

        private void UpdatesatifiedNumber()
        {
            if (clearStateNumber == 0)
            {
                State = true;
                animator.SetInteger("CircleState", 4);
                return;
            }
            satisfiedNumber = 0;

            foreach (var source in stateSources)
            {
                if (source.State)
                {
                    satisfiedNumber+=4/clearStateNumber;
                }
            }

            animator.SetInteger("CircleState", satisfiedNumber);

            if (satisfiedNumber == 4)
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
