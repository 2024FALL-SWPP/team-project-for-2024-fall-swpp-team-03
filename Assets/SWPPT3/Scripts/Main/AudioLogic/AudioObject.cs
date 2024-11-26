using UnityEngine;

namespace SWPPT3.Main.AudioLogic
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class AudioObject : MonoBehaviour
    {
        [SerializeField]
        protected AudioSource audioSource;

        public void SetVolume(float volume)
        {
            audioSource.volume = volume;
        }
    }
}
