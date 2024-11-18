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
        private Dictionary<StateSource, bool> circleStates;

        public void Awake()
        {
            circleStates = new Dictionary<StateSource, bool>();
            StateSource[] sources = GetComponentsInChildren<StateSource>();
            foreach (var source in sources)
            {
                circleStates[source] = false;
            }
        }

        protected override void OnSourceStateChanged(StateSource src, bool state)
        {
            circleStates[src] = state;
            _progress = 0;
            foreach (bool circleState in circleStates.Values)
            {
                if(circleState) _progress ++;
            }

            if (_progress >= circleStates.Count)
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
