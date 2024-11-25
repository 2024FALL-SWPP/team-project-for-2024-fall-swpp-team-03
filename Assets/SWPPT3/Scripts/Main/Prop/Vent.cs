using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class Vent : StateDst
    {
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private Collider collider;

        private float _colliderDisableDelay = 0.2f; // collider 비활성화까지 걸리는 시간
        private float _stateCooldownDelay = 1f; // vent안에 box가 일정 거리까지 내려가는데 걸리는 시간
        private float _offCooldownDelay = 0.2f; // 닫힐때 걸리는 시간

        private bool _isCooldownActive = false;

        protected override void OnSourceStateChanged(StateSource src, bool state)
        {
            State = state;
            if (_isCooldownActive)
                return;

            animator.SetBool("IsActive", state);
            if (state)
            {
                Invoke(nameof(DisableCollider), _colliderDisableDelay);
            }
            else
            {
                collider.enabled = true;
                Debug.Log("Collider has been enabled.");
            }

            _isCooldownActive = true;

            float cooldown = state ? _stateCooldownDelay : _offCooldownDelay;

            Invoke(nameof(ResetCooldown), cooldown);

            if (src.State != state)
            {
                Invoke(nameof(RepeatStateCheck), cooldown);
            }
        }

        private void DisableCollider()
        {
            if (animator.GetBool("IsActive"))
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
