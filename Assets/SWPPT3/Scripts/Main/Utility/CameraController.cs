using UnityEngine;
using SWPPT3.Main.Manager;
using SWPPT3.Main.Utility;

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

        private bool _isLeftButton = false;
        private bool _isRightButton = false;

        private void Start()
        {
            if (player == null) return;
            Vector3 initialPosition = player.position - player.forward * camerascript.DistanceFromPlayer + Vector3.up * camerascript.CameraHeight;
            transform.position = initialPosition;

            transform.LookAt(player);

            _currentRotation = transform.eulerAngles;

            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnLook += HandleLook;
                InputManager.Instance.OnStartRotation += HandleStartRotation;
                InputManager.Instance.OnStartTransform += HandleStartTransform;
            }
            else
            {
                Debug.LogError("InputManager 인스턴스가 존재하지 않습니다.");
            }
        }

        private void Update()
        {
            if (player == null) return;
            if (!_isLeftButton && !_isRightButton)
            {
                float mouseX = _lookInput.x * _mouseSensitivity;
                float mouseY = _lookInput.y * _mouseSensitivity;

                _currentRotation.x = Mathf.Clamp(_currentRotation.x - mouseY, -30f, 60f);
                _currentRotation.y += mouseX;

                Quaternion globalRotation = Quaternion.Euler(_currentRotation.x, _currentRotation.y, 0);
                Vector3 offset = globalRotation * new Vector3(0, camerascript.CameraHeight, -camerascript.DistanceFromPlayer);

                transform.position = player.position + offset;
                transform.LookAt(player);
            }
            _mouseSensitivity = camerascript.MouseSensitivity;
        }

        private void HandleLook(Vector2 lookInput)
        {
            _lookInput = lookInput;
        }

        private void HandleStartRotation(bool isRightButtonPressed)
        {
            _isRightButton = isRightButtonPressed;
        }

        private void HandleStartTransform(bool isLeftButtonPressed)
        {
            _isLeftButton = isLeftButtonPressed;
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnLook -= HandleLook;
                InputManager.Instance.OnStartRotation -= HandleStartRotation;
                InputManager.Instance.OnStartTransform -= HandleStartTransform;
            }
        }
    }
}
