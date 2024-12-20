#region

using SWPPT3.Main.Manager;
using UnityEngine;

#endregion

namespace SWPPT3.Main.Utility
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Transform player;

        private float _mouseSensitivity;

        [SerializeField]
        private CameraScript camerascript;

        private Vector3 _currentRotation;

        private Vector2 _lookInput;

        private bool _isRightButton = false;

        [SerializeField] private LayerMask cameraCollision;

        private void Start()
        {
            if (player != null)
            {
                Vector3 initialPosition = player.position - player.forward * camerascript.DistanceFromPlayer + Vector3.up * camerascript.CameraHeight;
                transform.position = initialPosition;

                transform.LookAt(player);

                _currentRotation = transform.eulerAngles;
            }

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnLook += HandleLook;
                InputManager.Instance.OnStartRotation += HandleStartRotation;
            }
            else
            {
                Debug.LogError("InputManager 인스턴스가 존재하지 않습니다.");
            }
        }

        private void Update()
        {
            if (player == null) return;
            if (!_isRightButton && GameManager.Instance.GameState == GameState.Playing)
            {
                float mouseX = _lookInput.x * _mouseSensitivity;
                float mouseY = _lookInput.y * _mouseSensitivity;

                _currentRotation.x = Mathf.Clamp(_currentRotation.x - mouseY, -40f, 60f);
                _currentRotation.y += mouseX;

                Quaternion globalRotation = Quaternion.Euler(_currentRotation.x, _currentRotation.y, 0);
                Vector3 offset = globalRotation * new Vector3(0, camerascript.CameraHeight, -camerascript.DistanceFromPlayer);

                transform.position = player.position + offset;
                transform.LookAt(player);

                Vector3 rayDir = transform.position - player.position;
                if (Physics.Raycast(player.position, rayDir, out RaycastHit hit, offset.magnitude , cameraCollision))
                {
                    transform.position = hit.point - rayDir.normalized;
                }
            }
            _mouseSensitivity = InputManager.Instance.CameraSensitivity;
        }

        private void HandleLook(Vector2 lookInput)
        {
            if (GameManager.Instance.GameState == GameState.Playing)
            {
                _lookInput = lookInput;
            }
        }

        private void HandleStartRotation(bool isRightButtonPressed)
        {
            _isRightButton = isRightButtonPressed;
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnLook -= HandleLook;
                InputManager.Instance.OnStartRotation -= HandleStartRotation;
            }

        }
    }
}
