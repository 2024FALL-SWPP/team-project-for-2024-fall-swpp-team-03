using UnityEngine;
using System.Collections.Generic;

namespace SWPPT3.Main.Prop
{
    public class FloorButton : StateSource
    {
        [SerializeField] private Animator animator;

        private HashSet<GameObject> _collidedObjects = new HashSet<GameObject>();

        private void OnTriggerEnter(Collider other)
        {
            _collidedObjects.Add(other.gameObject);
            State = On;
            animator.SetBool("IsPressed", true);
            //Debug.Log("FloorButton"+State);
        }

        private void OnTriggerExit(Collider other)
        {
            _collidedObjects.Remove(other.gameObject);
            if(_collidedObjects.Count == 0)
            {
                State = Off;
                animator.SetBool("IsPressed", false);
            }
            //Debug.Log("FloorButton"+State);
        }
    }
}
