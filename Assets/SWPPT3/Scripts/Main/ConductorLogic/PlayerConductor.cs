using System;
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

        private SoftbodyGenerator _softbody;

        private PlayerStates _previousState;

        public void OnEnable()
        {
            _softbody = GetComponent<SoftbodyGenerator>();
            if (_softbody != null)
            {
                _softbody.HandleCollisionEnterEvent += HandleCollisionEnter;
                _softbody.HandleCollisionExitEvent += HandleCollisionExit;
            }
        }

        public void OnDisable()
        {
            if (_softbody != null)
            {
                _softbody.HandleCollisionEnterEvent -= HandleCollisionEnter;
                _softbody.HandleCollisionExitEvent -= HandleCollisionExit;
            }
        }

        private void Awake()
        {
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

        private void HandleCollisionEnter(Collision other)
        {
            var conductor = other.gameObject.GetComponent<Conductor>();
            if (conductor == null) return;

            Connections.Add(conductor.gameObject);
            ConductorManager.IsDirty = true;
            //Debug.Log("Oncollisionenter"+ gameObject.name);
        }

        private void HandleCollisionExit(Collision other)
        {
            var conductor = other.gameObject.GetComponent<Conductor>();
            if (conductor == null) return;
            Connections.Remove(conductor.gameObject);
            ConductorManager.IsDirty = true;
        }
    }
}
