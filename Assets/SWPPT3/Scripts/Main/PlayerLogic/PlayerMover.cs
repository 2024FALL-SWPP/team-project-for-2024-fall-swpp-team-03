using System;
using System.Collections.Generic;
using SWPPT3.Main.Manager;
using SWPPT3.Main.PlayerLogic.State;
using SWPPT3.Main.Prop;
using SWPPT3.SoftbodyPhysics;
using Unity.Collections;
using UnityEngine;

namespace SWPPT3.Main.PlayerLogic
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private PlayerScript _playerScript;
        private Player _player;
        private SoftbodyGenerator _softbody;

        private float _moveSpeed;
        private float _jumpForce;
        private float _rotationSpeed;

        private HashSet<GameObject> _groundedObjects= new HashSet<GameObject>();

        [SerializeField] private Transform _cameraTransform;

        private Vector2 _moveVector;
        private Vector2 _lookInput;
        private Transform _playerTransform;

        private bool isRightButton = false;

        private void Start()
        {
            _player = GetComponent<Player>();
            _softbody = GetComponent<SoftbodyGenerator>();

            _moveSpeed = _playerScript.MoveSpeed;
            _rotationSpeed = _playerScript.RotationSpeed;
            _jumpForce = _playerScript.JumpForce;
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
            _softbody.move(force);

            if (isRightButton && _lookInput != Vector2.zero)
            {
                RotatePlayer();
            }
        }

        public void OnEnable()
        {
            Physics.ContactModifyEvent += ModificationEvent;
            if(_softbody != null)
            {
                _softbody.HandleCollisionEnterEvent += HandleCollisionEnter;
                _softbody.HandleCollisionStayEvent += HandleCollisionStay;
                _softbody.HandleCollisionExitEvent += HandleCollisionExit;
                _softbody.HandleTriggerEnterEvent += HandleTriggerEnter;
                _softbody.HandleTriggerExitEvent += HandleTriggerExit;
            }
        }

        public void OnDisable()
        {
            Physics.ContactModifyEvent -= ModificationEvent;
            if (_softbody != null)
            {
                _softbody.HandleCollisionEnterEvent -= HandleCollisionEnter;
                _softbody.HandleCollisionStayEvent -= HandleCollisionStay;
                _softbody.HandleCollisionExitEvent -= HandleCollisionExit;
                _softbody.HandleTriggerEnterEvent -= HandleTriggerEnter;
                _softbody.HandleTriggerExitEvent -= HandleTriggerExit;
            }
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
            if (_groundedObjects.Count > 0 && GameManager.Instance.GameState == GameState.Playing)
            {
                _softbody.SoftbodyJump(_jumpForce);
            }
            _softbody.IsJumpKey = true;
        }

        private void HandleJumpCancel()
        {
            _softbody.IsJumpKey = false;
        }

        private void HandleStartRotation(bool isRightButtonPressed)
        {
            isRightButton = isRightButtonPressed;
        }

        private void HandleLook(Vector2 lookInput)
        {
            _lookInput = lookInput;
        }

        private void HandleCollisionEnter(Collision collision)
        {
            var obstacle = collision.gameObject.GetComponent<PropBase>();
            if (obstacle != null)
            {
                _player.InteractWithProp(obstacle);
            }
        }
        private void HandleCollisionStay(Collision collision)
        {
            var isGroundedObject = false;
            foreach(var contactPoint in collision.contacts)
            {
                if (contactPoint.normal.y > _playerScript.Normalcriteria)
                {
                    isGroundedObject = true;
                }
            }
            if(isGroundedObject)
            {
                _groundedObjects.Add(collision.gameObject);
            }
            else
            {
                _groundedObjects.Remove(collision.gameObject);
            }
        }
        private void HandleCollisionExit(Collision collision)
        {
            _groundedObjects.Remove(collision.gameObject);
            var obstacle = collision.gameObject.GetComponent<PropBase>();
            if (obstacle != null)
            {
                _player.StopInteractWithProp(obstacle);
            }
        }

        private void HandleTriggerEnter(Collider other)
        {
            var obstacle = other.gameObject.GetComponent<PropBase>();
            if (obstacle != null)
            {
                _player.InteractWithProp(obstacle);
            }
        }

        private void HandleTriggerExit(Collider other)
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
