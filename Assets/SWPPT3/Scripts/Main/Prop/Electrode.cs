using SWPPT3.Main.PlayerLogic;
using SWPPT3.Main.PlayerLogic.State;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class Electrode : StatelessProp
    {
        private ElectroWire _parentWire;

        void Start()
        {
            if (_parentWire == null)
            {
                _parentWire = GetComponentInParent<ElectroWire>();
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            var other = collision.gameObject.GetComponent<ElectroBox>();
            if (other == null)
            {
                var player = collision.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    _parentWire.OnElectrodeCollisionEnter(this.gameObject, player.gameObject);
                }
            }
            else
            {
                _parentWire.OnElectrodeCollisionEnter(this.gameObject, collision.gameObject);
            }

        }

        void OnCollisionExit(Collision collision)
        {
            var other = collision.gameObject.GetComponent<ElectroBox>();
            if (other == null)
            {
                var player = collision.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    _parentWire.OnElectrodeCollisionExit(this.gameObject, player.gameObject);
                }
            }
            else
            {
                _parentWire.OnElectrodeCollisionExit(this.gameObject, collision.gameObject);
            }
        }
    }
}
