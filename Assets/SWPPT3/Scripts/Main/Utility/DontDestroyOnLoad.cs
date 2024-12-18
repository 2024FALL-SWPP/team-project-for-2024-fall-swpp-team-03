#region

using UnityEngine;

#endregion

namespace SWPPT3.Main.Utility
{
    [DisallowMultipleComponent]
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
