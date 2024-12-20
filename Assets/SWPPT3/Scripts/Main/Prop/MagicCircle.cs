#region

using UnityEngine;

#endregion

namespace SWPPT3.Main.Prop
{
    public class MagicCircle : StateDst
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Collider collider;
        private int _satisfiedNumber;
        private int _clearStateNumber;

        public void Awake()
        {
            _clearStateNumber = stateSources.Count;
            UpdatesatifiedNumber();
        }

        protected override void OnSourceStateChanged(StateSource src, bool state)
        {
            UpdatesatifiedNumber();
        }

        private void UpdatesatifiedNumber()
        {
            if (_clearStateNumber == 0)
            {
                State = true;
                animator.SetInteger("CircleState", 4);
                return;
            }
            _satisfiedNumber = 0;

            foreach (var source in stateSources)
            {
                if (source.State)
                {
                    _satisfiedNumber+=4/_clearStateNumber;
                }
            }

            animator.SetInteger("CircleState", _satisfiedNumber);

            if (_satisfiedNumber == 4)
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
