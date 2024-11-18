using System;
using System.Collections.Generic;
using UnityEngine;
using SWPPT3.Main.Prop;
using UnityEngine.InputSystem;
using SWPPT3.Main.PlayerLogic.State;
using SWPPT3.Main.Utility;

namespace SWPPT3.Main.PlayerLogic
{
    public class Player : MonoBehaviour
    {
        private PlayerStates _currentState = PlayerStates.Slime;
        private Vector2 _inputMovement;
        private Vector3 _moveDirection;
        [SerializeField]
        private PlayerSettings settings;

        private bool _isGameOver = false;

        private bool _isHoldingJump;

        public bool IsGrounded
        {
            get;
            set;
        }

        [SerializeField]
        private Rigidbody _rb;

        private PhysicMaterial _physicMaterial;
        private Collider _collider;

        private HashSet<GameObject> _groundedObjects= new HashSet<GameObject>();

        public Dictionary<PlayerStates, int> Item = new()
        {
            { PlayerStates.Slime, 0 },
            { PlayerStates.Metal, 0 },
            { PlayerStates.Rubber, 0 },
        };

        private PlayerState PlayerState => _playerStates[_currentState];
        public PlayerStates CurrentState => _currentState;

        private PlayerInputActions _inputActions;

        private readonly Dictionary<PlayerStates, PlayerState> _playerStates = new()
        {
            { PlayerStates.Metal, new MetalState() },
            { PlayerStates.Rubber, new RubberState() },
            { PlayerStates.Slime, new SlimeState() },
        };

        public ElectroWire ElectroWireObject;

        private List<GameObject> _connectedElectroObjects = new List<GameObject>();
        public List<GameObject> GetConnectedObjects()
        {
            return _connectedElectroObjects;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _physicMaterial = _collider.material;
            _isHoldingJump = false;
            IsGrounded = false;
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
                    transform.Translate(Vector3.forward * (settings.moveSpeed * Time.deltaTime));
                }

                if (_currentState == PlayerStates.Rubber && !IsGrounded  && _isHoldingJump)
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
            if (IsGrounded)
            {
                _rb.AddForce(Vector3.up * settings.jumpForce, ForceMode.Impulse);

            }
            _isHoldingJump = true;
        }
        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            _isHoldingJump = false;
            if (_currentState == PlayerStates.Rubber)
            {
                _physicMaterial.bounciness = 0.5f;
                _physicMaterial.bounceCombine = PhysicMaterialCombine.Maximum;

                _collider.material = _physicMaterial;
            }
        }

        public void OnChangeState(InputAction.CallbackContext context)
        {
            if (_isGameOver) return;
            string keyPressed = context.control.displayName;

            PlayerStates newState = keyPressed switch
            {
                "1" => PlayerStates.Slime,
                "2" => PlayerStates.Metal,
                "3" => PlayerStates.Rubber,
                _ => _currentState
            };
            if (newState == PlayerStates.Slime || Item[newState] > 0)
            {
                if(newState != PlayerStates.Slime) Item[newState]--;
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
            var contactPoint = collision.contacts[0];
            if (contactPoint.normal.y > settings.groundThreshold)
            {
                _groundedObjects.Add(collision.gameObject);
                IsGrounded = true;
            }

            var electroObject = collision.gameObject.GetComponent<ElectroBox>();
            if (electroObject != null)
            {
                _connectedElectroObjects.Add(electroObject.gameObject);
                ElectroWireObject?.CheckConnection();
            }
            var obstacle = collision.gameObject.GetComponent<PropBase>();
            if (obstacle != null)
            {
                InteractWithProp(obstacle);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            _groundedObjects.Remove(collision.gameObject);
            IsGrounded = _groundedObjects.Count > 0;
            var electroObject = collision.gameObject.GetComponent<ElectroBox>();
            if (electroObject != null)
            {
                _connectedElectroObjects.Remove(electroObject.gameObject);
                ElectroWireObject?.CheckConnection();
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
            settings.jumpForce = 2.0f * _rb.mass;
        }
    }
}
