#region

using SWPPT3.Main.Manager;
using UnityEngine;

#endregion

namespace SWPPT3.Main.AudioLogic
{
    public class BgmObject : AudioObject
    {
        public bool BgmState { get; set; }

        [SerializeField]
        private AudioLowPassFilter audioLowPassFilter;

        void Awake()
        {
            BgmManager.Instance.AddBgmObject(this);
            if (audioLowPassFilter == null)
            {
                audioLowPassFilter.cutoffFrequency = 22000f;
            }

        }

        public void OnDestroy()
        {
            BgmManager.Instance.RemoveBgmObject(this);
        }

        public override void PlaySound()
        {
            audioSource.Play();
        }

        public override void StopSound()
        {
            audioSource.Stop();
        }

        public void SetPitch(float pitch)
        {
            audioSource.pitch = pitch;
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
