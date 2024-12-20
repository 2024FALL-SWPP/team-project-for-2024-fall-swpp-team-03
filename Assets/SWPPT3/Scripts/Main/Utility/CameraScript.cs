#region

using UnityEngine;

#endregion

namespace SWPPT3.Main.Utility
{
    [CreateAssetMenu(fileName = "CameraScript", menuName = "SWPPT3/Scripts/CameraScript")]
    public class CameraScript : ScriptableObject
    {
        [SerializeField] private float mouseSensitivity;
        [SerializeField] private float distanceFromPlayer ;
        [SerializeField] private float cameraHeight ;
        [SerializeField] private float rayCastOffset;
        public float MouseSensitivity { get => mouseSensitivity; set => mouseSensitivity = value; }
        public float DistanceFromPlayer{ get => distanceFromPlayer; set => distanceFromPlayer = value; }
        public float CameraHeight{ get => cameraHeight; set => cameraHeight = value; }
        public float RayCastOffset { get => rayCastOffset; set => rayCastOffset = value; }

    }
}
