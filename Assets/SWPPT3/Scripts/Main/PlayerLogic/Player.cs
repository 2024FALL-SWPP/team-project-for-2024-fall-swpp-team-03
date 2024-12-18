#region

using System;
using System.Collections.Generic;
using SWPPT3.Main.PlayerLogic.State;
using SWPPT3.Main.Prop;
using SWPPT3.SoftbodyPhysics;
using UnityEngine;

#endregion

namespace SWPPT3.Main.PlayerLogic
{
    public class Player : MonoBehaviour
    {
        private PlayerStates _currentState = PlayerStates.Slime;
        private Vector2 _inputMovement;

        public Dictionary<PlayerStates, int> Item;

        private SoftbodyGenerator _softbody;

        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Material _slimeMaterial;
        [SerializeField] private Material _rubberMaterial;
        [SerializeField] private Material _metalMaterial;



        public event Action OnItemChanged;

        private PlayerState PlayerState => _playerStates[_currentState];

        public PlayerStates CurrentState => _currentState;


        private readonly Dictionary<PlayerStates, PlayerState> _playerStates = new()
        {
            { PlayerStates.Metal, new MetalState() },
            { PlayerStates.Rubber, new RubberState() },
            { PlayerStates.Slime, new SlimeState() },
        };

        private void Awake()
        {
            _softbody = GetComponent<SoftbodyGenerator>();
            if (Item == null)
            {
                Item = new Dictionary<PlayerStates, int>
                {
                    { PlayerStates.Slime, 0 },
                    { PlayerStates.Metal, 0 },
                    { PlayerStates.Rubber, 0 },
                };
            }
            OnItemChanged?.Invoke();
            TryChangeState(PlayerStates.Slime);

        }

        public void TryChangeState(PlayerStates newState)
        {
            if (newState == PlayerStates.Slime || Item[newState] > 0)
            {
                if (newState != PlayerStates.Slime) Item[newState]--;
                OnItemChanged?.Invoke();
                _currentState = newState;
                if (newState == PlayerStates.Rubber)
                {
                    _meshRenderer.material = _rubberMaterial;
                    _softbody.SetRubber();
                }
                else if (newState == PlayerStates.Metal)
                {
                    _meshRenderer.material = _metalMaterial;
                    _softbody.SetMetal();
                }
                else if (newState == PlayerStates.Slime)
                {
                    _meshRenderer.material = _slimeMaterial;
                    _softbody.SetSlime();
                }
            }

            Debug.Log(_currentState);
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
        }
        public void SetItemCounts(int newSlimeCount, int newMetalCount, int newRubberCount)
        {
            if (Item == null)
            {
                Item = new Dictionary<PlayerStates, int>
                {
                    { PlayerStates.Slime, 0 },
                    { PlayerStates.Metal, 0 },
                    { PlayerStates.Rubber, 0 },
                };
            }
            OnItemChanged?.Invoke();
            Item[PlayerStates.Slime] = newSlimeCount;
            Item[PlayerStates.Metal] = newMetalCount;
            Item[PlayerStates.Rubber] = newRubberCount;
        }
    }
}
