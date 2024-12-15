using SWPPT3.Main.Utility.Singleton;
using UnityEngine;
using UnityEngine.Audio;

namespace SWPPT3.Main.Manager
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [SerializeField]
        private AudioMixer _mainMixer;
    }
}
