using System;
using UnityEngine;

namespace SWPPT3.Main.Utility
{
    [DisallowMultipleComponent]
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void OnValidate()
        {
            if (transform.parent != null)
            {
                Debug.Log("DDOL should be placed on root object!");
            }
        }
    }
}
