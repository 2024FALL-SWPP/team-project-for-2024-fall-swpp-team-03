using System.Collections.Generic;
using SWPPT3.Main.Manager;
using SWPPT3.Main.PlayerLogic.State;
using SWPPT3.Main.Prop;
using UnityEngine;

namespace SWPPT3.Main.PlayerLogic
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField]
        private PlayerScript _playerScript;
        [SerializeField]
        private Player _player;
        [SerializeField]
        private Rigidbody _rb;

        [SerializeField]
        private float _moveSpeed;
        [SerializeField]
        private float _jumpMultiplier;

        private float _jumpForce;
        [SerializeField]
        private float _rotationSpeed;

        private HashSet<GameObject> _groundedObjects= new HashSet<GameObject>();

        [SerializeField]
        private Transform _cameraTransform;

        private Vector2 _moveVector;
        private Vector2 _lookInput;
        private bool _isGrounded;
        private Transform _playerTransform;

        private bool isRightButton = false;
        private bool _isHoldingJump;

        private void Start()
        {
            _isHoldingJump = false;
            _isGrounded = false;
            _playerTransform = transform;
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnMove += HandleMove;
                InputManager.Instance.OnJump += HandleJump;
                InputManager.Instance.OnJumpCancel += HandleJumpCancel;
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
            _moveSpeed = _playerScript.MoveSpeed;
            _jumpMultiplier = _playerScript.JumpMultiplier;
            _rotationSpeed = _playerScript.RotationSpeed;

            UpdateJumpForce();
            Vector3 moveDirection = GetMoveDirection();
            Vector3 force = moveDirection * _moveSpeed;
            _rb.AddForce(force, ForceMode.VelocityChange);

            if (isRightButton && _lookInput != Vector2.zero)
            {
                RotatePlayer();
            }
            if (_player.CurrentState == PlayerStates.Rubber && !_isGrounded  && _isHoldingJump)
            {
                _player.SetBounciness(1.0f);
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
            _isHoldingJump = true;
        }

        private void HandleJumpCancel()
        {
            _isHoldingJump = false;
            if (_player.CurrentState == PlayerStates.Rubber)
            {
                _player.SetBounciness(0.5f);
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

        public void UpdateJumpForce()
        {
            _jumpForce = _jumpMultiplier * _player.Rigidbody.mass;
        }

        private void OnCollisionEnter(Collision collision)
        {

            var contactPoint = collision.contacts[0];
            if (contactPoint.normal.y > 0.7f)
            {
                _groundedObjects.Add(collision.gameObject);
                _isGrounded = true;
            }
            var obstacle = collision.gameObject.GetComponent<PropBase>();
            if (obstacle != null)
            {
                _player.InteractWithProp(obstacle);
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            _groundedObjects.Remove(collision.gameObject);
            _isGrounded = _groundedObjects.Count > 0;
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnMove -= HandleMove;
                InputManager.Instance.OnJump -= HandleJump;
                InputManager.Instance.OnJumpCancel -= HandleJumpCancel;
                InputManager.Instance.OnStartRotation -= HandleStartRotation;
                InputManager.Instance.OnLook -= HandleLook;
            }
        }
    }
}
