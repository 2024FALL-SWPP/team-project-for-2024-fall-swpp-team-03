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

        [SerializeField]
        private AudioLowPassFilter bgmLowPassFilter; // 추가된 필드

        [Header("Audio Mixer")]
        [SerializeField]
        private AudioMixer masterMixer;
        [SerializeField]
        private string bgmMixerGroupName = "BGM";

        private List<BgmObject> bgmObjects;

        private float _bgmVolume;
        public float BGMVolume { get => _bgmVolume; set => _bgmVolume = value; }
        private float _sfxVolume;
        public float SFXVolume { get => _sfxVolume; set => _sfxVolume = value; }

        private List<SfxObject> sfxObjects;

        public void AddBgmObject(BgmObject bgm)
        {
            if (bgm != null && !bgmObjects.Contains(bgm))
            {
                bgmObjects.Add(bgm);
            }
        }

        public void RemoveBgmObject(BgmObject bgm)
        {
            if (bgm != null && bgmObjects.Contains(bgm))
            {
                bgmObjects.Remove(bgm);
            }
        }

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
            bgmObjects = new List<BgmObject>();
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
        }

        public void Update()
        {
            SetBGMVolume(BGMVolume);
            SetSFXVolume(SFXVolume);
            var state = GameManager.Instance.GameState;
            switch (state)
            {
                case GameState.Playing:
                case GameState.Ready:
                case GameState.BeforeStart:
                case GameState.Exit:
                case GameState.OnOption:
                    ApplyLowPassFilter(false);
                    PlayBGM();
                    break;
                case GameState.Paused:
                    ApplyLowPassFilter(true);
                    break;
                case GameState.GameOver:
                    PlayFailSound();
                    break;
                case GameState.StageCleared:
                    PlaySuccessSound();
                    break;
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

        public void BgmSourcesStop()
        {
            foreach (var sound in bgmObjects)
            {
                sound.StopSound();
            }
        }

        public void PlayFailSound()
        {
            BgmSourcesStop();
            failSound.PlayOneShot(failSound.clip);
        }

        public void PlaySuccessSound()
        {
            BgmSourcesStop();
            successSound.PlayOneShot(successSound.clip);
        }

        public void SetBGMVolume(float volume)
        {
            bgmSource.volume = volume;
            foreach (AudioObject audioObject in bgmObjects)
            {
                audioObject.SetVolume(volume);
            }
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
