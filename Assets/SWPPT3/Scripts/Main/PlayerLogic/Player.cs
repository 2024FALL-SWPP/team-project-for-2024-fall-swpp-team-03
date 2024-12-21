#region

using System;
using System.Collections.Generic;
using SWPPT3.Main.AudioLogic;
using SWPPT3.Main.Manager;
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

        private PlayerSound _playerSound;

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
            _playerSound = GetComponent<PlayerSound>();
            _playerSound.PlaySound(Sounds.Awake);
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
                if (_currentState != newState)
                {
                    if (newState == PlayerStates.Slime)
                    {
                        _playerSound.PlaySound(Sounds.Slime);
                    }
                    else
                    {
                        _playerSound.PlaySound(Sounds.Change);
                    }
                }
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

        public void ItemSound()
        {
            _playerSound.PlaySound(Sounds.Item);
        }

        public void CollisionSound()
        {
            _playerSound.PlaySound(Sounds.Collision);
        }

        public void InteractWithProp(PropBase prop)
        {
            PlayerState.InteractWithProp(this, prop);
        }

        public void StopInteractWithProp(PropBase prop)
        {
            PlayerState.StopInteractWithProp(this, prop);
        }

        public void PlusItemCounts(PlayerStates itemState)
        {
            Item[itemState]++;
            OnItemChanged?.Invoke();
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

            Item[PlayerStates.Slime] = newSlimeCount;
            Item[PlayerStates.Metal] = newMetalCount;
            Item[PlayerStates.Rubber] = newRubberCount;

            OnItemChanged?.Invoke();
        }
    }
}
