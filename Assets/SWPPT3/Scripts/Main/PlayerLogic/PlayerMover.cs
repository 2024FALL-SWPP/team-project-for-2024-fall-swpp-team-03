using System;
using System.Collections.Generic;
using SWPPT3.Main.Manager;
using SWPPT3.Main.PlayerLogic.State;
using SWPPT3.Main.Prop;
using Unity.Collections;
using UnityEngine;

namespace SWPPT3.Main.PlayerLogic
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private PlayerScript _playerScript;
        [SerializeField] private Player _player;
        [SerializeField] private Rigidbody _rb;

        private float _moveSpeed;
        private float _jumpForce;
        private float _rotationSpeed;

        private HashSet<GameObject> _groundedObjects= new HashSet<GameObject>();

        [SerializeField] private Transform _cameraTransform;

        private Vector2 _moveVector;
        private Vector2 _lookInput;
        private bool _isGrounded;
        private Transform _playerTransform;

        private bool isRightButton = false;
        private bool _isHoldingJump;

        private void Start()
        {
            _moveSpeed = _playerScript.MoveSpeed;
            _rotationSpeed = _playerScript.RotationSpeed;
            _jumpForce = _playerScript.JumpForce * _rb.mass;
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

        public void OnEnable()
        {
            Physics.ContactModifyEvent += ModificationEvent;
        }

        public void OnDisable()
        {
            Physics.ContactModifyEvent -= ModificationEvent;
        }

        public void ModificationEvent(PhysicsScene scene, NativeArray<ModifiableContactPair> pairs)
        {
            for (int i = 0; i < pairs.Length; i++)
            {
                var pair = pairs[i];

                var properties = pair.massProperties;

                properties.inverseMassScale = 1f;
                properties.inverseInertiaScale = 1f;
                properties.otherInverseMassScale = 0;
                properties.otherInverseInertiaScale = 0;

                pair.massProperties = properties;

                pairs[i] = pair;
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
            // Debug.Log("Rotate Player");
            float rotationInput = _lookInput.x * _rotationSpeed * Time.deltaTime;
            _playerTransform.Rotate(Vector3.up, rotationInput, Space.World);
        }

        private void HandleMove(Vector2 moveVector)
        {
            _moveVector = moveVector;
        }

        private void HandleJump()
        {
            if (GameManager.Instance.GameState == GameState.Playing)
            {
                if (_isGrounded)
                {
                    _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
                }
                _isHoldingJump = true;
                if (_player.CurrentState == PlayerStates.Rubber)
                {
                    _player.SetBounciness(1.0f);
                }
            }
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
        }

        private void HandleLook(Vector2 lookInput)
        {
            _lookInput = lookInput;
        }


        private void OnCollisionEnter(Collision collision)
        {

            var contactPoint = collision.contacts[0];
            if (contactPoint.normal.y > _playerScript.Normalcriteria)
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
            var obstacle = collision.gameObject.GetComponent<PropBase>();
            if (obstacle != null)
            {
                _player.StopInteractWithProp(obstacle);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var obstacle = other.gameObject.GetComponent<PropBase>();
            if (obstacle != null)
            {
                _player.InteractWithProp(obstacle);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var obstacle = other.gameObject.GetComponent<PropBase>();
            if (obstacle != null)
            {
                _player.StopInteractWithProp(obstacle);
            }
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
