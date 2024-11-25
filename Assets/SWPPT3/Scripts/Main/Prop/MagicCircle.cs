using System;
using UnityEngine;
using System.Collections.Generic;

namespace SWPPT3.Main.Prop
{
    public class MagicCircle : StateDst
    {
        [SerializeField]
        private Animator animator;

        private Dictionary<StateSource, string> sourceToParamMap;

        public void Awake()
        {
            sourceToParamMap = new Dictionary<StateSource, string>();

            for (int i = 0; i < stateSources.Count; i++)
            {
                string parameterName = $"IsActive{i + 1}";
                sourceToParamMap[stateSources[i]] = parameterName;
            }
        }

        protected override void OnSourceStateChanged(StateSource src, bool state)
        {

            animator.SetBool(sourceToParamMap[src], state);

            if (CheckActive())
            {
                ActivateMagicCircle();
            }
        }

        private bool CheckActive()
        {
            foreach (StateSource source in stateSources)
            {
                if (!source.State)
                {
                    State = false;
                    return false;
                }
            }
            State = true;
            return true;
        }

        private void ActivateMagicCircle()
        {
            Debug.Log("Magic Circle Activated");
        }
    }
}
