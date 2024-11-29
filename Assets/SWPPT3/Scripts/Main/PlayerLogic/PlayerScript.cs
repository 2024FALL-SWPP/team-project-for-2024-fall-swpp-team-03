using UnityEngine;

namespace SWPPT3.Main.PlayerLogic
{
    [CreateAssetMenu(fileName = "PlayerScript", menuName = "SWPPT3/Scripts/PlayerScript")]
    public class PlayerScript : ScriptableObject
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float rotationSpeed;

        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
        public float JumpForce { get => jumpForce; set => jumpForce = value; }
        public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
    }
}
