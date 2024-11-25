using System;
using System.Collections.Generic;
using SWPPT3.Main.Manager;
using UnityEngine;
using SWPPT3.Main.Prop;
using UnityEngine.InputSystem;
using SWPPT3.Main.PlayerLogic.State;

namespace SWPPT3.Main.PlayerLogic
{
    public class Player : MonoBehaviour
    {
        private PlayerStates _currentState = PlayerStates.Slime;
        private Vector2 _inputMovement;

        private bool _isGameOver = false;

        [SerializeField]
        private Rigidbody _rb;
        [SerializeField]
        public PhysicMaterial _physicMaterial;
        [SerializeField]
        public Collider _collider;

        public Dictionary<PlayerStates, int> Item = new()
        {
            { PlayerStates.Slime, 0 },
            { PlayerStates.Metal, 0 },
            { PlayerStates.Rubber, 0 },
        };

        private PlayerState PlayerState => _playerStates[_currentState];

        public PlayerStates CurrentState => _currentState;

        public Rigidbody Rigidbody => _rb;

        private readonly Dictionary<PlayerStates, PlayerState> _playerStates = new()
        {
            { PlayerStates.Metal, new MetalState() },
            { PlayerStates.Rubber, new RubberState() },
            { PlayerStates.Slime, new SlimeState() },
        };
        public void SetBounciness(float bounciness, PhysicMaterialCombine bounceCombine = PhysicMaterialCombine.Average)
        {
            _physicMaterial.bounciness = bounciness;
            _physicMaterial.bounceCombine = bounceCombine;

            _collider.material = _physicMaterial;
        }

        private void Awake()
        {
            InputManager.Instance.OnChangeState += HandleChangeState;
        }


        private void Update()
        {
            if (_isGameOver)
            {

            }
            else
            {

            }
        }

        public void HandleChangeState(InputAction.CallbackContext context)
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

            TryChangeState(newState);
        }

        public void TryChangeState(PlayerStates newState)
        {
            if (newState == PlayerStates.Slime || Item[newState] > 0)
            {
                if (newState != PlayerStates.Slime) Item[newState]--;

                PlayerState.ChangeRigidbody(_rb);
                PlayerState.ChangePhysics(_collider, _physicMaterial);
            }
        }

        public void InteractWithProp(PropBase prop)
        {
            PlayerState.InteractWithProp(this, prop);
        }



        public void GameOver()
        {
            _isGameOver = true;
            Debug.Log("Game Over");
        }
        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnChangeState -= HandleChangeState;
            }
        }


    }
}
