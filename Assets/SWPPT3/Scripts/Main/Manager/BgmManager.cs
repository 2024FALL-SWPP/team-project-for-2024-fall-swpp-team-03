using SWPPT3.Main.Utility.Singleton;
using UnityEngine;
using UnityEngine.Audio; // AudioMixer를 사용하기 위해 필요
using System.Collections.Generic;
using SWPPT3.Main.AudioLogic;
using UnityEngine.Serialization;

namespace SWPPT3.Main.Manager
{
    public class BgmManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField]
        private AudioSource bgmSource;

        [SerializeField] private AudioSource failSound;
        [SerializeField] private AudioSource successSound;



        [SerializeField]
        private AudioLowPassFilter bgmLowPassFilter; // 추가된 필드

        [Header("Audio Mixer")]
        [SerializeField]
        private AudioMixer masterMixer;
        [SerializeField]
        private string bgmMixerGroupName = "BGM";

        [SerializeField]
        private List<BgmObject> bgmObjects;

        [SerializeField]
        private List<SfxObject> sfxObjects;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (masterMixer != null)
            {
                AudioMixerGroup[] mixerGroups = masterMixer.FindMatchingGroups(string.Empty);

                foreach (var group in mixerGroups)
                {
                    if (group.name == bgmMixerGroupName)
                    {
                        bgmSource.outputAudioMixerGroup = group;
                    }
                }
            }
        }

        #region BGM Methods

        public void PlayBGM()
        {
            bgmSource.Play();
        }

        public void StopBGM()
        {
            bgmSource.Stop();
        }

        public void PlayFailSound()
        {
            failSound.PlayOneShot(failSound.clip);
        }

        public void PlaySuccessSound()
        {
            successSound.PlayOneShot(successSound.clip);
        }

        public void SetVolume(float volume)
        {
            bgmSource.volume = volume;
            foreach (AudioObject audioObject in bgmObjects)
            {
                audioObject.SetVolume(volume);
            }
            foreach (AudioObject audioObject in sfxObjects)
            {
                audioObject.SetVolume(volume);
            }
        }

        #endregion

        #region Low-Pass Filter Control

        public void ApplyLowPassFilter(bool apply, float cutoffFrequency = 500f, float transitionTime = 1.0f)
        {
            if (apply)
            {
                masterMixer.FindSnapshot("LowPass").TransitionTo(transitionTime);

                bgmLowPassFilter.enabled = true;
                bgmLowPassFilter.cutoffFrequency = cutoffFrequency;

                foreach (BgmObject bgmObject in bgmObjects)
                {
                    bgmObject.ApplySlow(true);
                }
            }
            else
            {
                masterMixer.FindSnapshot("Normal").TransitionTo(transitionTime);

                bgmLowPassFilter.enabled = false;

                foreach (BgmObject bgmObject in bgmObjects)
                {
                    bgmObject.ApplySlow(false);
                }
            }
        }

        #endregion
    }
}
