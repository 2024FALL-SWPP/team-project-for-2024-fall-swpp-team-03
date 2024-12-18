#region

using System;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

#endregion

namespace SWPPT3.Main.Utility.Singleton
{
    public abstract class MonoWeakSingleton : MonoBehaviour { }

    public abstract class MonoWeakSingleton<T> : MonoWeakSingleton where T : MonoWeakSingleton<T>
    {
        private static T s_instance;

        private static readonly object _mutex = new();

        [CanBeNull]
        public static T Instance
        {
            get
            {
                if (s_instance != null) return s_instance;

                lock (_mutex)
                {
                    var findResult = FindObjectsOfType<T>();

                    switch (findResult.Length)
                    {
                        case 0:
                            Debug.LogWarning($"[{nameof(MonoWeakSingleton)}]: No instance of {nameof(T)} found. Returning null.");
                            return null;

                        case > 1:
                            Debug.LogWarning($"[{nameof(MonoWeakSingleton)}]: Multiple instances of {nameof(T)} found. Returning first, ignoring others.");
                            return s_instance = findResult[0];

                        case 1:
                            return s_instance = findResult[0];

                        default:
                            throw new ArgumentOutOfRangeException(
                                nameof(findResult.Length),
                                $"{nameof(FindObjectOfType)} returned negative length array");
                    }
                }
            }
        }

        private void OnDestroy()
        {
            lock (_mutex)
            {
                s_instance = null;
            }
        }
    }
}
