using System;
using System.Collections.Generic;
using UnityEngine;

namespace SWPPT3.Main.ConductorLogic
{
    public class Conductor : MonoBehaviour
    {
        protected ConductorManager conductorManager;

        private List<GameObject> _connections;

        public List<GameObject> GetConnections()
        {
            return _connections;
        }

        public bool CurrentFlow;

        public virtual bool IsConductive()
        {
            return true;
        }

        public virtual bool IsCurrentFlow()
        {
            return CurrentFlow;
        }

        private void Awake()
        {
            _connections = new List<GameObject>();
            CurrentFlow = false;
        }

        private void OnCollisionEnter(Collision other)
        {
            var conductor = other.gameObject.GetComponent<Conductor>();
            if (conductor == null) return;

            _connections.Add(conductor.gameObject);
            conductorManager.IsDirty = true;
        }

        private void OnCollisionExit(Collision other)
        {
            var conductor = other.gameObject.GetComponent<Conductor>();
            if (conductor == null) return;
            _connections.Remove(conductor.gameObject);
            conductorManager.IsDirty = true;
        }
    }
}
