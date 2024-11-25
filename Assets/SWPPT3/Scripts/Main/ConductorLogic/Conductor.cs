using System;
using System.Collections.Generic;
using UnityEngine;
using SWPPT3.Main.ConductorLogic;

namespace SWPPT3.Main.ConductorLogic
{
    public class Conductor : MonoBehaviour
    {

        public List<GameObject> Connections { get ; set; }


        public virtual bool IsConductive()
        {
            return true;
        }

        private void Awake()
        {
            Connections = new List<GameObject>();
        }

        private void OnCollisionEnter(Collision other)
        {
            var conductor = other.gameObject.GetComponent<Conductor>();
            if (conductor == null) return;

            Connections.Add(conductor.gameObject);
            ConductorManager.IsDirty = true;
        }

        private void OnCollisionExit(Collision other)
        {
            var conductor = other.gameObject.GetComponent<Conductor>();
            if (conductor == null) return;
            Connections.Remove(conductor.gameObject);
            ConductorManager.IsDirty = true;
        }
    }
}
