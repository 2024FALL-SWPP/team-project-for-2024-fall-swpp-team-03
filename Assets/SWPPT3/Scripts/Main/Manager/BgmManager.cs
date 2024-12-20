#region

using System.Collections.Generic;
using SWPPT3.Main.AudioLogic;
using SWPPT3.Main.Utility.Singleton;
using UnityEngine;
using UnityEngine.Audio;
// AudioMixer를 사용하기 위해 필요

#endregion

namespace SWPPT3.Main.Manager
{
    public class BgmManager : MonoSingleton<BgmManager>
    {
        [Header("Audio Sources")]
        [SerializeField]
        private AudioSource bgmSource;

        [SerializeField] private AudioSource failSound;
        [SerializeField] private AudioSource successSound;

        [Header("Audio Mixer")]
        [SerializeField]
        private AudioMixer masterMixer;
        [SerializeField]
        private string bgmMixerGroupName = "BGM";

        private float _bgmVolume;
        public float BGMVolume { get => _bgmVolume; set => _bgmVolume = value; }
        private float _sfxVolume;
        public float SFXVolume { get => _sfxVolume; set => _sfxVolume = value; }

        private List<SfxObject> sfxObjects;

        public void AddSfxObject(SfxObject sfx)
        {
            if (sfx != null && !sfxObjects.Contains(sfx))
            {
                sfxObjects.Add(sfx);
            }
        }

        public void RemoveSfxObject(SfxObject sfx)
        {
            if (sfx != null && sfxObjects.Contains(sfx))
            {
                sfxObjects.Remove(sfx);
            }
        }


        private void Awake()
        {
            sfxObjects = new List<SfxObject>();
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

            bgmSource.Play();
        }

        public void Update()
        {
            SetBGMVolume(BGMVolume);
            SetSFXVolume(SFXVolume);
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
            failSound.Play();
        }

        public void StopFailSound()
        {
            failSound.Stop();
        }

        public void StopSuccessSound()
        {
            successSound.Stop();
        }

        public void PlaySuccessSound()
        {
            successSound.Play();
        }

        public void StopAllBGM()
        {
            bgmSource.Stop();
            successSound.Stop();
            failSound.Stop();
        }

        public void SetBGMVolume(float volume)
        {
            bgmSource.volume = volume;
            failSound.volume = volume;
            successSound.volume = volume;
        }

        public void SetSFXVolume(float volume)
        {
            foreach (var audioObject in sfxObjects)
            {
                audioObject.SetVolume(volume);
            }
        }

        #endregion


    }
}




