using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.PlayerLogic.State;
using UnityEngine;

namespace SWPPT3.Main.ConductorLogic
{
    public class PlayerConductor : Conductor
    {
        [SerializeField]
        private Player _player;

        private PlayerStates _previousState;

        private void Awake()
        {
            _previousState = _player.CurrentState;
        }

        private void Update()
        {
            if (_player.CurrentState != _previousState)
            {
                _previousState = _player.CurrentState;
                conductorManager.IsDirty = true;
            }
        }

        public override bool IsConductive()
        {
            return _player.CurrentState == PlayerStates.Metal;
        }
    }
}
