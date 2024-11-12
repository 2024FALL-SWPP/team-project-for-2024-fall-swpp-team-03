using System;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public abstract class StateDst : StatefulProp
    {
        [SerializeField]
        private StateSource stateSource;
        private void OnEnable()
        {
            stateSource.OnStateChanged += OnSourceStateChanged;
        }
        private void OnDisable()
        {
            stateSource.OnStateChanged -= OnSourceStateChanged;
        }
        protected abstract void OnSourceStateChanged(bool state);

    }
}
