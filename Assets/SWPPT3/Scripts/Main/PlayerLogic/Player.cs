using System;
using System.Collections.Generic;
using UnityEngine;
using SWPPT3.Main.Prop;
using UnityEngine.InputSystem;
using SWPPT3.Main.PlayerLogic.State;

namespace SWPPT3.Main.PlayerLogic
{
    public class Player : MonoBehaviour
    {
        private States _currentState = States.Slime;
        private Vector2 _inputMovement;
        private Vector3 _moveDirection;
        private float _moveSpeed = 4f;

        private bool _isGameOver = false;

        private bool _isJumping;
        private bool _isHoldingJump;

        [SerializeField]
        private Rigidbody _rb;

        private PhysicMaterial _physicMaterial;
        private Collider _collider;
        private float _jumpForce = 2f;

        public Dictionary<States, int> Item = new()
        {
            { States.Slime, 0 },
            { States.Metal, 0 },
            { States.Rubber, 0 },
        };

        private PlayerState PlayerState => _playerStates[_currentState];

        private PlayerInputActions _inputActions;

        private readonly Dictionary<States, PlayerState> _playerStates = new()
        {
            { States.Metal, new MetalState() },
            { States.Rubber, new RubberState() },
            { States.Slime, new SlimeState() },
        };

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _physicMaterial = _collider.material;
            _isHoldingJump = false;
            _inputActions = new PlayerInputActions();
            _inputActions.PlayerActions.Move.performed += OnMove;
            _inputActions.PlayerActions.Move.canceled += OnMove;
            _inputActions.PlayerActions.Jump.performed += OnJump;
            _inputActions.PlayerActions.Jump.canceled += OnJumpCanceled;
            _inputActions.PlayerActions.ChangeState.performed += OnChangeState;
        }

        private void OnEnable()
        {
            _inputActions.PlayerActions.Move.Enable();
            _inputActions.PlayerActions.Jump.Enable();
            _inputActions.PlayerActions.ChangeState.Enable();
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.PlayerActions.Move.Disable();
            _inputActions.PlayerActions.Jump.Disable();
            _inputActions.PlayerActions.ChangeState.Disable();
            _inputActions.Disable();
        }

        private void Update()
        {
            if (_isGameOver)
            {

            }
            else
            {
                if (_moveDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(_moveDirection);
                    transform.Translate(Vector3.forward * (_moveSpeed * Time.deltaTime));
                }

                if (_currentState == States.Rubber && !IsGrounded() && _isHoldingJump)
                {
                    _physicMaterial.bounciness = 1.0f;
                    _physicMaterial.bounceCombine = PhysicMaterialCombine.Maximum;

                    _collider.material = _physicMaterial;
                }
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            _moveDirection = input != Vector2.zero ? new Vector3(input.x, 0f, input.y) : Vector3.zero;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (_isGameOver) return;
            if (IsGrounded())
            {
                _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            }
            _isHoldingJump = true;
        }
        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            _isHoldingJump = false;
            if (_currentState == States.Rubber)
            {
                _physicMaterial.bounciness = 0.5f;
                _physicMaterial.bounceCombine = PhysicMaterialCombine.Maximum;

                _collider.material = _physicMaterial;
            }
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position, Vector3.down, 1f);
        }

        public void OnChangeState(InputAction.CallbackContext context)
        {
            if (_isGameOver) return;
            string keyPressed = context.control.displayName;

            States newState = keyPressed switch
            {
                "1" => States.Slime,
                "2" => States.Metal,
                "3" => States.Rubber,
                _ => _currentState
            };
            if (newState == States.Slime || Item[newState] > 0)
            {
                if(newState != States.Slime) Item[newState]--;
                _currentState = newState;
                PlayerState.ChangeRigidbody(_rb);
                PlayerState.ChangePhysics(_collider, _physicMaterial);
                UpdateJumpForce();
                // Debug.Log($"New state: {newState} mass: {_rb.mass} kg");
            }
        }

        public void InteractWithProp(PropBase prop)
        {
            PlayerState.InteractWithProp(this, prop);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var obstacle = collision.gameObject.GetComponent<PropBase>();
            if (obstacle != null)
            {
                InteractWithProp(obstacle);
            }
        }

        public void GameOver()
        {
            _isGameOver = true;
            OnDisable();
            Debug.Log("Game Over");
        }

        public void UpdateJumpForce()
        {
            _jumpForce = 2.0f * _rb.mass;
        }
    }
}
