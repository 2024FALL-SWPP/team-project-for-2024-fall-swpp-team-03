using UnityEngine;
using SWPPT3.Main.Manager;

namespace SWPPT3.Main
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rb;

        [SerializeField]
        private float _jumpForce = 5f;

        [SerializeField]
        private float _rotationSpeed = 100f;

        [SerializeField]
        private float _groundCheckDistance = 1.1f;

        // [SerializeField]
        private Transform _cameraTransform;

        [SerializeField]
        private GameObject _cameraObject;

        private Vector2 _moveVector;
        private Vector2 _lookInput;
        private bool _isGrounded;
        private Transform _playerTransform;

        private bool isRightButton = false;

        private void Start()
        {
            _playerTransform = transform;
            _cameraTransform = _cameraObject.GetComponent<Transform>();
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnMove += HandleMove;
                InputManager.Instance.OnJump += HandleJump;
                InputManager.Instance.OnStartRotation += HandleStartRotation;
                InputManager.Instance.OnLook += HandleLook;
            }
            else
            {
                Debug.LogError("InputManager 인스턴스가 존재하지 않습니다.");
            }
        }

        private void Update()
        {
            Vector3 moveDirection = GetMoveDirection();
            Vector3 force = moveDirection * 0.1f;
            _rb.AddForce(force, ForceMode.VelocityChange);

            _isGrounded = Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance);

            if (isRightButton && _lookInput != Vector2.zero)
            {
                RotatePlayer();
            }
        }

        private Vector3 GetMoveDirection()
        {
            Vector3 forward = _cameraTransform.forward;
            Vector3 right = _cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = forward * _moveVector.y + right * _moveVector.x;
            return desiredMoveDirection;
        }

        private void RotatePlayer()
        {
            float rotationInput = _lookInput.x * _rotationSpeed * Time.deltaTime;
            _playerTransform.Rotate(Vector3.up, rotationInput, Space.World);
        }

        private void HandleMove(Vector2 moveVector)
        {
            _moveVector = moveVector;
            Debug.Log($"PlayerMover - Move: {_moveVector}");
        }

        private void HandleJump()
        {
            if (_isGrounded)
            {
                _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
                Debug.Log("PlayerMover - Jump performed");
            }
        }

        private void HandleStartRotation(bool isRightButtonPressed)
        {
            isRightButton = isRightButtonPressed;
            Debug.Log($"PlayerMover - StartRotation: {isRightButton}");
        }

        private void HandleLook(Vector2 lookInput)
        {
            _lookInput = lookInput;
            Debug.Log($"PlayerMover - Look Input: {_lookInput}");
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnMove -= HandleMove;
                InputManager.Instance.OnJump -= HandleJump;
                InputManager.Instance.OnStartRotation -= HandleStartRotation;
                InputManager.Instance.OnLook -= HandleLook;
            }
        }
    }
}
