using UnityEngine;

namespace SWPPT3.Main.AudioLogic
{
    public class BgmPlayer : AudioPlayer
    {
        [SerializeField]
        private AudioLowPassFilter audioLowPassFilter;

        void Awake()
        {
            audioLowPassFilter.cutoffFrequency = 22000f;
        }

        public void PlaySound()
        {
            AudioSource.Play();
        }

        public void StopSound()
        {
            AudioSource.Stop();
        }

        public void SetPitch(float pitch)
        {
            AudioSource.pitch = pitch;
        }

        public void ApplyLowPassFilter(bool apply, float cutoffFrequency = 500f)
        {
            if (apply)
            {
                audioLowPassFilter.cutoffFrequency = cutoffFrequency;
            }
            else
            {
                audioLowPassFilter.cutoffFrequency = 22000f;
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
