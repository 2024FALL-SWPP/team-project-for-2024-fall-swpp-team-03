using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SWPPT3.Main.Prop
{
    public class MagicCircle : StateDst
    {
        [SerializeField]
        private int stateChangeAmount;

        private int _progress = 0;
        private Dictionary<StateSource, bool> _circleStates;

        public void Awake()
        {
            _circleStates = new Dictionary<StateSource, bool>();
            StateSource[] sources = GetComponentsInChildren<StateSource>();
            foreach (var source in sources)
            {
                _circleStates[source] = false;
            }
        }

        protected override void OnSourceStateChanged(StateSource src, bool state)
        {
            _circleStates[src] = state;
            _progress = 0;
            foreach (bool circleState in _circleStates.Values)
            {
                if(circleState) _progress ++;
            }

            if (_progress >= _circleStates.Count)
            {
                ActivateMagicCircle();
                State = On;
            }
        }

        private void ActivateMagicCircle()
        {
            Debug.Log("Magic Circle Activated");
        }
    }
}
