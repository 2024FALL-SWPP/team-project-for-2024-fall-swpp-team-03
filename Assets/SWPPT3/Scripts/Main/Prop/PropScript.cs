#region

using UnityEngine;

#endregion

namespace SWPPT3.Main.Utility
{
    [CreateAssetMenu(fileName = "PropScript", menuName = "SWPPT3/Scripts/PropScript")]
    public class PropScript : ScriptableObject
    {
        [SerializeField] private float _colliderDisableDelay = 0.2f; // collider 비활성화까지 걸리는 시간
        [SerializeField] private float _stateCooldownDelay = 1f; // vent안에 box가 일정 거리까지 내려가는데 걸리는 시간
        [SerializeField] private float _offCooldownDelay = 0.2f; // 닫힐때 걸리는 시간

        [SerializeField] private float _buoyancyCoefficient; // 또는 config파일로 관리
        [SerializeField] private float _dampingFactor = 0.5f;
        [SerializeField] private float _clampFactor = 3.0f;
        public float ColliderDisableDelay { get=> _colliderDisableDelay; set => _colliderDisableDelay = value; }
        public float StateCooldownDelay { get => _stateCooldownDelay; set => _stateCooldownDelay = value; }
        public float OffCooldownDelay { get => _offCooldownDelay; set => _offCooldownDelay = value; }

        public float BuoyancyCoefficient { get => _buoyancyCoefficient; set => _buoyancyCoefficient = value; }
        public float DampingFactor { get => _dampingFactor; set => _dampingFactor = value; }
        public float ClampFactor { get => _clampFactor; set => _clampFactor = value; }


    }
}
