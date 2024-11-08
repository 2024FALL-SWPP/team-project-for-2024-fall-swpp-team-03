using System;
using System.Collections.Generic;
using UnityEngine;
using SWPPT3.Main.Prop;
using UnityEngine.InputSystem;
using SWPPT3.Main.PlayerLogic.State;

namespace SWPPT3.Main.Player
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        private States _currentState = States.Slime;
        private Vector2 _inputMovement;
        private Vector3 _moveDirection;
        private float _moveSpeed = 4f;
        public States CurrentState => _currentState;

        [SerializeField]
        private Rigidbody _rb;
        public float jumpForce = 5f;

        private PlayerState PlayerState => _playerStates[_currentState];

        private PlayerInputActions _inputActions; // New Input System 추가

        private readonly Dictionary<States, PlayerState> _playerStates = new()
        {
            { States.Metal, new MetalState() },
            { States.Rubber, new RubberState() },
            { States.Slime, new SlimeState() },
        };

        private void Awake()
        {
            // 싱글톤 패턴 설정
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            _rb = GetComponent<Rigidbody>();
            _inputActions = new PlayerInputActions();
            _inputActions.PlayerActions.Move.performed += OnMove;
            _inputActions.PlayerActions.Move.canceled += OnMove;
            _inputActions.PlayerActions.Jump.performed += OnJump;
        }

        private void OnEnable()
        {
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }

        private void Update()
        {
            // 이동 방향이 있을 때만 이동
            if (_moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(_moveDirection);
                transform.Translate(Vector3.forward * (_moveSpeed * Time.deltaTime));
            }
        }

        // 이동 입력 처리
        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            _moveDirection = input != Vector2.zero ? new Vector3(input.x, 0f, input.y) : Vector3.zero;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (IsGrounded()) // 바닥에 있을 때만 점프
            {
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            }
        }

        private bool IsGrounded()
        {
            // 간단한 바닥 체크 예시
            return Physics.Raycast(transform.position, Vector3.down, 1.1f);
        }

        public void ChangeState(States state)
        {
            _currentState = state;
        }

        public void InteractWithObject(PropBase prop)
        {
            PlayerState.InteractWithProp(prop);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var obstacle = collision.gameObject.GetComponent<PropBase>();
            if (obstacle != null)
            {
                InteractWithObject(obstacle);
            }
        }
    }
}
