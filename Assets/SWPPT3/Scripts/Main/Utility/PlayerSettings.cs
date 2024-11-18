using UnityEngine;

namespace SWPPT3.Main.Utility
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Utility/PlayerSettings")]
    public class PlayerSettings : ScriptableObject
    {
        public float groundThreshold = 0.7f;
        public float moveSpeed = 4f;
        public float jumpForce = 2f;
    }
}
