using System;
using System.Collections.Generic;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public abstract class StateDst : StatefulProp
    {
        [SerializeField]
        private List<StateSource> stateSources;
        private void OnEnable()
        {
            foreach (StateSource stateSource in stateSources)
            {
                stateSource.OnStateChanged += OnSourceStateChanged;
            }

        }
        private void OnDisable()
        {
            foreach (StateSource stateSource in stateSources)
            {
                stateSource.OnStateChanged -= OnSourceStateChanged;
            }
        }
        protected abstract void OnSourceStateChanged(StateSource src, bool state);

    }
}
