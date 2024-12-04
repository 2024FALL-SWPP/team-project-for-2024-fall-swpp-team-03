using UnityEngine;

namespace SWPPT3.Main.PlayerLogic
{
    [CreateAssetMenu(fileName = "PlayerScript", menuName = "SWPPT3/Scripts/PlayerScript")]
    public class PlayerScript : ScriptableObject
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _rotationSpeed;

        [SerializeField] private float _normalcriteria;

        public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
        public float JumpForce { get => _jumpForce; set => _jumpForce = value; }
        public float RotationSpeed { get => _rotationSpeed; set => _rotationSpeed = value; }
        public float Normalcriteria { get => _normalcriteria; set => _normalcriteria = value; }
    }
}
