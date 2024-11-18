using System.Collections.Generic;
using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.PlayerLogic.State;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class ElectroBox : StatelessProp
    {
        public ElectroWire ElectroWireObject;

        protected List<GameObject> connectedObjects = new List<GameObject>();

        public List<GameObject> GetConnectedObjects()
        {
            return connectedObjects;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var other = collision.gameObject.GetComponent<ElectroBox>();
            if (other != null)
            {
                connectedObjects.Add(other.gameObject);
                ElectroWireObject?.CheckConnection();
            }
            else
            {
                var player = collision.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    connectedObjects.Add(player.gameObject);
                    ElectroWireObject?.CheckConnection();
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            var other = collision.gameObject.GetComponent<ElectroBox>();
            if (other != null)
            {
                connectedObjects.Remove(other.gameObject);
                ElectroWireObject?.CheckConnection();
            }
            else
            {
                var player = collision.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    connectedObjects.Remove(player.gameObject);
                    ElectroWireObject?.CheckConnection();
                }
            }
        }

    }
}
