using UnityEngine;

namespace SWPPT3.Main.AudioLogic
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioLowPassFilter))]
    public class AudioObject : MonoBehaviour
    {
        [SerializeField]
        private AudioSource audioSource;

        private AudioLowPassFilter lowPassFilter;

        void Awake()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            lowPassFilter = GetComponent<AudioLowPassFilter>();
            lowPassFilter.cutoffFrequency = 22000f;
        }

        public void PlaySound(bool loop = false)
        {
            audioSource.loop = loop;
            audioSource.Play();
        }

        public void StopSound()
        {
            audioSource.Stop();
        }

        public void SetVolume(float volume)
        {
            audioSource.volume = volume;
        }

        public void SetPitch(float pitch)
        {
            audioSource.pitch = pitch;
        }

        public void ApplyLowPassFilter(bool apply, float cutoffFrequency = 500f)
        {
            if (apply)
            {
                lowPassFilter.cutoffFrequency = cutoffFrequency;
            }
            else
            {
                lowPassFilter.cutoffFrequency = 22000f;
            }
        }

        public void ApplySlow(bool apply,float pitch = 0.2f, float cutoffFrequency = 500f)
        {
            if (apply)
            {
                ApplyLowPassFilter(true, cutoffFrequency);
                SetPitch(pitch);
            }
            else
            {
                ApplyLowPassFilter(false);
                SetPitch(1.0f);
            }
        }
    }
}
