#region

using System;
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

        private List<AudioObject> audioObjects;

        public void AddSfxObject(AudioObject audioObject)
        {
            if (audioObject != null && !audioObjects.Contains(audioObject))
            {
                audioObjects.Add(audioObject);
            }
        }

        public void RemoveSfxObject(AudioObject audioObject)
        {
            if (audioObject != null && audioObjects.Contains(audioObject))
            {
                audioObjects.Remove(audioObject);
            }
        }


        private void Awake()
        {
            _bgmVolume = 0.5f;
            _sfxVolume = 0.5f;
            audioObjects = new List<AudioObject>();

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
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }

        public void OnGameStateChanged(GameState oldState, GameState newState)
        {
            switch (newState)
            {
                case GameState.BeforeStart:
                    if (oldState == GameState.OnOption || oldState == GameState.Exit || oldState == GameState.OnHowto) break;
                    StopAllBGM();
                    PlayBGM();
                    break;
                case GameState.Playing:
                    StopAllBGM();
                    break;

                case GameState.GameOver:
                    PlayFailSound();
                    break;

                case GameState.StageCleared:
                    PlaySuccessSound();
                    break;

                default:
                    break;
            }
        }

        public void Update()
        {
            SetBGMVolume(BGMVolume);
            SetSFXVolume(SFXVolume);
        }

        public void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
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
            foreach (var audioObject in audioObjects)
            {
                audioObject.SetVolume(volume);
            }
        }

        #endregion


    }
}




