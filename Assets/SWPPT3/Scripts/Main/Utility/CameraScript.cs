using UnityEngine;

namespace SWPPT3.Main.Utility
{
    [CreateAssetMenu(fileName = "CameraScript", menuName = "SWPPT3/Scripts/CameraScript")]
    public class CameraScript : ScriptableObject
    {
        [SerializeField] private float _mouseSensitivity;
        public float MouseSensitivity
        {
            get => _mouseSensitivity;
            set => _mouseSensitivity = value;
        }
    }
}
