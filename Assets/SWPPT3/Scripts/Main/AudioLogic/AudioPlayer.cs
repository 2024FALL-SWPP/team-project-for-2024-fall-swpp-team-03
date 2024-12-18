using UnityEngine;

namespace SWPPT3.Main.AudioLogic
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class AudioPlayer : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audioSource;

        protected AudioSource AudioSource => _audioSource;

        public void SetVolume(float volume)
        {
            AudioSource.volume = volume;
        }

        private void OnValidate()
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }
}
