using SWPPT3.Main.Generated;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SWPPT3.Main.Manager
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        private AlcheslimeInput _inputActions;

        public delegate void MoveAction(Vector2 moveVector);
        public event MoveAction OnMove;

        public delegate void JumpAction();
        public event JumpAction OnJump;
        public delegate void JumpCancelAction();
        public event JumpCancelAction OnJumpCancel;

        public delegate void LookAction(Vector2 lookInput);
        public event LookAction OnLook;

        public delegate void StartRotationAction(bool isRightButton);
        public event StartRotationAction OnStartRotation;

        public delegate void StartTransformAction(bool isLeftButton);
        public event StartTransformAction OnStartTransform;

        //나중에 제거될 함수
        public delegate void ChangeStateAction(InputAction.CallbackContext context);
        public event ChangeStateAction OnChangeState;
        //

        private void Awake()
        {
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

            _inputActions = new AlcheslimeInput();

            var inGame = _inputActions.InGame;
            inGame.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
            inGame.Move.canceled += ctx => OnMove?.Invoke(Vector2.zero);
            inGame.Jump.performed += ctx => OnJump?.Invoke();
            inGame.Jump.canceled += ctx => OnJumpCancel?.Invoke();
            inGame.Look.performed += ctx => OnLook?.Invoke(ctx.ReadValue<Vector2>());
            inGame.Look.canceled += ctx => OnLook?.Invoke(Vector2.zero);
            inGame.StartRotation.performed += ctx => OnStartRotation?.Invoke(true);
            inGame.StartRotation.canceled += ctx => OnStartRotation?.Invoke(false);
            inGame.StartTransform.performed += ctx => OnStartTransform?.Invoke(true);
            inGame.StartTransform.canceled += ctx => OnStartTransform?.Invoke(false);
            //나중에 제거
            inGame.ChangeState.performed += ctx => OnChangeState?.Invoke(ctx);
            //
        }

        private void OnDestroy()
        {
            if (_inputActions != null)
            {
                _inputActions.Disable();
            }
        }

        private void OnEnable()
        {
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }
    }
}
