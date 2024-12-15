using UnityEngine;

namespace SWPPT3.Main.Utility
{
    [CreateAssetMenu(fileName = "CameraScript", menuName = "SWPPT3/Scripts/CameraScript")]
    public class CameraScript : ScriptableObject
    {
        [SerializeField] private float mouseSensitivity;
        [SerializeField] private float distanceFromPlayer = 10f;
        [SerializeField] private float cameraHeight = 2f;
        public float MouseSensitivity { get => mouseSensitivity; set => mouseSensitivity = value; }
        public float DistanceFromPlayer{ get => distanceFromPlayer; set => distanceFromPlayer = value; }
        public float CameraHeight{ get => cameraHeight; set => cameraHeight = value; }

    }
}
