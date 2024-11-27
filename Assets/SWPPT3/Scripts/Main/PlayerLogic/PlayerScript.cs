using UnityEngine;

namespace SWPPT3.Main.PlayerLogic
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Utility/PlayerSettings")]
    public class PlayerScript : ScriptableObject
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float jumpMultiplier;
        [SerializeField] private float rotationSpeed;

        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
        public float JumpMultiplier { get => jumpMultiplier; set => jumpMultiplier = value; }
        public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
    }
}
