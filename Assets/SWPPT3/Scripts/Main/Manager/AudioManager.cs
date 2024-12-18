#region

using SWPPT3.Main.Utility.Singleton;
using UnityEngine;
using UnityEngine.Audio;

#endregion

namespace SWPPT3.Main.Manager
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private enum AudioSnapshot
        {
            Lobby,
            InGame,
            Pause,
        }

        [SerializeField]
        private AudioMixer _mainMixer;

    }
}
