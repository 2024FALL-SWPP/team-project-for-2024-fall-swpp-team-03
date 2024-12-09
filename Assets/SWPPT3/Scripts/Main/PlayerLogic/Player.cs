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
        private int slimeCount , metalCount, rubberCount;
        public Dictionary<PlayerStates, int> Item;

        [SerializeField]
        private Rigidbody _rb;
        // [SerializeField]
        public PhysicMaterial _physicMaterial;
        [SerializeField]
        public Collider _collider;

        private PlayerState PlayerState => _playerStates[_currentState];

        public PlayerStates CurrentState => _currentState;

        public Rigidbody Rigidbody => _rb;

        private readonly Dictionary<PlayerStates, PlayerState> _playerStates = new()
        {
            { PlayerStates.Metal, new MetalState() },
            { PlayerStates.Rubber, new RubberState() },
            { PlayerStates.Slime, new SlimeState() },
        };
        public void SetBounciness(float bounciness, PhysicMaterialCombine bounceCombine = PhysicMaterialCombine.Maximum)
        {
            _physicMaterial.bounciness = bounciness;
            _physicMaterial.bounceCombine = bounceCombine;

            _collider.material = _physicMaterial;
        }

        private void Awake()
        {
            TryChangeState(PlayerStates.Slime);
            InputManager.Instance.OnChangeState += HandleChangeState;
            Item = new Dictionary<PlayerStates, int>
            {
                { PlayerStates.Slime, slimeCount },
                { PlayerStates.Metal, metalCount },
                { PlayerStates.Rubber, rubberCount },
            };
        }




        public void HandleChangeState(InputAction.CallbackContext context)
        {
            string keyPressed = context.control.displayName;
            Debug.Log(keyPressed);

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
                _currentState = newState;
                PlayerState.ChangeRigidbody(_rb);
                PlayerState.ChangePhysics(_collider, _physicMaterial);
                _collider.hasModifiableContacts = newState == PlayerStates.Slime;
            }
        }

        public void InteractWithProp(PropBase prop)
        {
            PlayerState.InteractWithProp(this, prop);
        }

        public void StopInteractWithProp(PropBase prop)
        {
            PlayerState.StopInteractWithProp(this, prop);
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnChangeState -= HandleChangeState;
            }
        }
        public void SetItemCounts(int newSlimeCount, int newMetalCount, int newRubberCount)
        {
            slimeCount = newSlimeCount;
            metalCount = newMetalCount;
            rubberCount = newRubberCount;

            Item[PlayerStates.Slime] = slimeCount;
            Item[PlayerStates.Metal] = metalCount;
            Item[PlayerStates.Rubber] = rubberCount;
        }
    }
}
