#region

using System.Collections.Generic;
using System.Linq;
using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.PlayerLogic.State;
using SWPPT3.SoftbodyPhysics;
using UnityEngine;

#endregion

namespace SWPPT3.Main.ConductorLogic
{
    public class PlayerConductor : Conductor
    {
        [SerializeField]
        private Player _player;
        private SoftbodyGenerator _softbodygenerator;

        private SoftbodyGenerator _softbody;

        private PlayerStates _previousState;

        public override IEnumerable<GameObject> GetConnections => _connectedObjects.AsEnumerable();

        private readonly HashSet<GameObject> _connectedObjects = new();
        private readonly Dictionary<int, bool> _stayMap = new();
        private readonly HashSet<int> _removeSet = new();

        public void OnEnable()
        {
            _softbody = GetComponent<SoftbodyGenerator>();
            if (_softbody != null)
            {
                _softbody.HandleCollisionEnterEvent += HandleCollisionEnter;
                _softbody.HandleCollisionExitEvent += HandleCollisionExit;
                _softbody.HandleCollisionStayEvent += HandleCollisionStay;
            }
        }

        public void OnDisable()
        {
            if (_softbody != null)
            {
                _softbody.HandleCollisionEnterEvent -= HandleCollisionEnter;
                _softbody.HandleCollisionExitEvent -= HandleCollisionExit;
                _softbody.HandleCollisionStayEvent -= HandleCollisionStay;
            }
        }

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

            _removeSet.Clear();

            foreach (var kvp in _stayMap)
            {
                if (!kvp.Value)
                {
                    _removeSet.Add(kvp.Key);
                }
            }

            _connectedObjects.RemoveWhere(go => _removeSet.Contains(go.GetInstanceID()));

            foreach (var k in _removeSet)
            {
                _stayMap.Remove(k);
            }

            Debug.Log(_connectedObjects.Count);

            if (_removeSet.Count != 0)
            {
                ConductorManager.IsDirty = true;
            }

            foreach (var connectedObject in _connectedObjects)
            {
                _stayMap[connectedObject.GetInstanceID()] = false;
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

            _connectedObjects.Add(conductor.gameObject);
            _stayMap[conductor.gameObject.GetInstanceID()] = true;
            ConductorManager.IsDirty = true;
            //Debug.Log("Oncollisionenter"+ gameObject.name);
        }

        private void HandleCollisionStay(Collision other)
        {
            var conductor = other.gameObject.GetComponent<Conductor>();
            if (conductor == null) return;

            _stayMap[conductor.gameObject.GetInstanceID()] = true;
        }

        private void HandleCollisionExit(Collision other)
        {
            // var conductor = other.gameObject.GetComponent<Conductor>();
            // if (conductor == null) return;
            //
            //
            // conductor.gameObject.id
            // if (Connections.Contains(conductor.gameObject))
            //     Connections.Remove(conductor.gameObject);
            //
            // ConductorManager.IsDirty = true;
        }
    }
}
