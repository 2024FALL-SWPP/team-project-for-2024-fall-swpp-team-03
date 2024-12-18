#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace SWPPT3.Main.ConductorLogic
{
    public class Conductor : MonoBehaviour
    {
        public virtual IEnumerable<GameObject> GetConnections => Connections.AsEnumerable();

        public List<GameObject> Connections { get ; set; } = new ();


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
            var conductor = other.gameObject.GetComponentInParent<Conductor>();
            if (conductor == null) return;

            Connections.Add(conductor.gameObject);
            ConductorManager.IsDirty = true;
            //Debug.Log("Oncollisionenter"+ gameObject.name);
        }

        private void OnCollisionExit(Collision other)
        {
            var conductor = other.gameObject.GetComponentInParent<Conductor>();
            if (conductor == null) return;
            Connections.Remove(conductor.gameObject);
            ConductorManager.IsDirty = true;
        }
    }
}
