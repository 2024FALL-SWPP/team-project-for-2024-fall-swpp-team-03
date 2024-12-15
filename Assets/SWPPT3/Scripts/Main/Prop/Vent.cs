using SWPPT3.Main.Utility;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class Vent : StateDst
    {
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private Collider collider;

        [SerializeField] private PropScript _propscript;



        private bool _isCooldownActive = false;

        protected override void OnSourceStateChanged(StateSource src, bool state)
        {
            if (maintainState && State) return;
            State = state;
            if (_isCooldownActive)
                return;

            animator.SetBool("IsClosed", state);
            if (state)
            {
                Invoke(nameof(DisableCollider), _propscript.ColliderDisableDelay);
            }
            else
            {
                collider.enabled = true;
                Debug.Log("Collider has been enabled.");
            }

            _isCooldownActive = true;

            float cooldown = state ? _propscript.StateCooldownDelay : _propscript.OffCooldownDelay;

            Invoke(nameof(ResetCooldown), cooldown);

            if (src.State != state)
            {
                Invoke(nameof(RepeatStateCheck), cooldown);
            }
        }

        private void DisableCollider()
        {
            if (animator.GetBool("IsClosed"))
            {
                collider.enabled = false;
                Debug.Log("Collider has been disabled.");
            }
        }

        private void ResetCooldown()
        {
            _isCooldownActive = false;
            Debug.Log("Cooldown has ended. Function can be called again.");
        }

        private void RepeatStateCheck()
        {
            OnSourceStateChanged(null, State);
        }
    }
}
