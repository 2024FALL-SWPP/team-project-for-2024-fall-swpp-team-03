

using System;
using Codice.CM.Common;
using UnityEditor;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class Door : StateDst
    {
        [SerializeField]
        private Collider collider;

        [SerializeField]
        private Animator animator;

        private void Awake()
        {
            State = true;
        }

        protected override void OnSourceStateChanged(StateSource src, bool state)
        {
            state = !state;
            State = state;
            collider.enabled = state;
            animator.SetBool("IsClosed",state);
            Debug.Log("Door:");
            Debug.Log(state);
        }
    }
}
