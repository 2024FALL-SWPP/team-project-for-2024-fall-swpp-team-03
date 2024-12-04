using UnityEngine;
using SWPPT3.Main.Manager;
using SWPPT3.Main.Utility;

namespace SWPPT3.Main
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Transform _player;

        private float _mouseSensitivity;

        [SerializeField]
        private CameraScript _cameraScript;

        private float _distanceFromPlayer = 10f;
        private float _cameraHeight = 2f;
        private Vector3 _currentRotation;

        private Vector2 _lookInput;

        private bool _isLeftButton = false;
        private bool _isRightButton = false;

        private void Start()
        {
            Vector3 initialPosition = _player.position - _player.forward * _distanceFromPlayer + Vector3.up * _cameraHeight;
            transform.position = initialPosition;

            transform.LookAt(_player);

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
            if (!_isLeftButton && !_isRightButton)
            {
                float mouseX = _lookInput.x * _mouseSensitivity;
                float mouseY = _lookInput.y * _mouseSensitivity;

                _currentRotation.x = Mathf.Clamp(_currentRotation.x - mouseY, -30f, 60f);
                _currentRotation.y += mouseX;

                Quaternion globalRotation = Quaternion.Euler(_currentRotation.x, _currentRotation.y, 0);
                Vector3 offset = globalRotation * new Vector3(0, _cameraHeight, -_distanceFromPlayer);

                transform.position = _player.position + offset;
                transform.LookAt(_player);
            }
            _mouseSensitivity = _cameraScript.MouseSensitivity;
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
