#region

using System;
using UnityEngine;

#endregion

namespace SWPPT3.Main.Utility.Singleton
{
    [RequireComponent(typeof(DontDestroyOnLoad))]
    public abstract class MonoSingleton : MonoBehaviour { }

    public abstract class MonoSingleton<T>: MonoBehaviour where T: MonoSingleton<T>
    {
        private static readonly Lazy<T> s_instance = new(LocateSingletonObject);

        // ReSharper disable once StaticMemberInGenericType
        private static readonly object s_mutex = new();

        public static T Instance => s_instance.Value;

        private static T LocateSingletonObject()
        {
            lock (s_mutex)
            {
                var findResult = FindObjectsOfType<T>();

                switch (findResult.Length)
                {
                    case 0:
                        Debug.LogWarning($"[{nameof(MonoSingleton)}]: No instance of {nameof(T)} found in scene! Creating one as new...");

                        var newInstance = new GameObject().AddComponent<T>();
                        return newInstance;

                    case > 1:
                        Debug.LogWarning($"[{nameof(MonoSingleton)}]: Multiple instance of {nameof(T)} found in scene!");
                        Debug.LogWarning($"[{nameof(MonoSingleton)}]: Selecting first one, destroying others...");

                        var selected = findResult[0];

                        for (var i = 1; i < findResult.Length; i++)
                        {
                            Destroy(findResult[i]);
                        }

                        return selected;

                    case 1:
                        return findResult[0];

                    default:
                        throw new ArgumentOutOfRangeException(nameof(findResult.Length), $"[{nameof(MonoSingleton)}]: {nameof(FindObjectOfType)} returned negative length array");
                }
            }
        }
    }
}
