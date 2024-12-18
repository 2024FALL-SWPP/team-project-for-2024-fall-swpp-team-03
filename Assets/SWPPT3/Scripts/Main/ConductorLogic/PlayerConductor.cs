using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.PlayerLogic.State;
using SWPPT3.SoftbodyPhysics;
using UnityEngine;

namespace SWPPT3.Main.ConductorLogic
{
    public class PlayerConductor : Conductor
    {
        [SerializeField]
        private Player _player;
        private SoftbodyGenerator _softbodygenerator;

        private PlayerStates _previousState;

        private void Awake()
        {
            _softbodygenerator = gameObject.GetComponent<SoftbodyGenerator>();
            _previousState = _player.CurrentState;
        }

        private void Update()
        {
            if (_player.CurrentState != _previousState)
            {
                _previousState = _player.CurrentState;
                ConductorManager.IsDirty = true;
            }
        }

        public override bool IsConductive()
        {
            return _player.CurrentState == PlayerStates.Metal;
        }

        public void OncollisionEnter(Collision collision)
        {
            _softbodygenerator
        }
    }
}
