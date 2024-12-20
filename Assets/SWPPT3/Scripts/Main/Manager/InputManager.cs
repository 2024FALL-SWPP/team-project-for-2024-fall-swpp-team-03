#region

using SWPPT3.Main.Generated;
using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.Utility;
using SWPPT3.Main.Utility.Singleton;
using UnityEngine;

#endregion

namespace SWPPT3.Main.Manager
{
    public class InputManager : MonoSingleton<InputManager>
    {
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

        public delegate void EscAction();
        public event EscAction OnEsc;

        [SerializeField] CameraScript _cameraScript;
        [SerializeField] PlayerScript _playerScript;

        private float cameraSensitivity;
        private float cameraCoefficient;

        public float CameraCoffeicient
        {
            get { return cameraCoefficient; }
            set { cameraCoefficient = value; }
        }
        public float CameraSensitivity{ get { return cameraSensitivity * cameraCoefficient; } }

        private float rotationSensitivity;
        private float rotationCoefficient;

        public float RotationCoefficient
        {
            get { return rotationCoefficient; }
            set { rotationCoefficient = value; }
        }
        public float RotationSensitivity{get{return rotationSensitivity * rotationCoefficient;} }



        private void Awake()
        {
            cameraSensitivity = _cameraScript.MouseSensitivity;
            rotationSensitivity = _playerScript.RotationSpeed;

            cameraCoefficient = 0.5f;
            rotationCoefficient = 0.5f;
            // Input System 초기화
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

            inGame.EscMenu.performed += ctx => OnEsc?.Invoke();

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
