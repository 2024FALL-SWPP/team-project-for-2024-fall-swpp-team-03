using SWPPT3.Main.Utility.Singleton;
using UnityEngine;
using UnityEngine.Audio; // AudioMixer를 사용하기 위해 필요
using System.Collections.Generic;
using SWPPT3.Main.AudioLogic;

namespace SWPPT3.Main.Manager
{
    public class AudioManager : MonoWeakSingleton<AudioManager>
    {
        [Header("Audio Clips")]
        [SerializeField]
        private List<AudioClip> bgmClips;
        [SerializeField]
        private List<AudioClip> sfxClips;

        [Header("Audio Sources")]
        [SerializeField]
        private AudioSource bgmSource;
        [SerializeField]
        private AudioSource sfxSource;

        [Header("Audio Mixer")]
        [SerializeField]
        private AudioMixer masterMixer;
        [SerializeField]
        private string bgmMixerGroupName = "BGM";
        [SerializeField]
        private string sfxMixerGroupName = "SFX";

        private Dictionary<string, AudioClip> bgmClipDict = new Dictionary<string, AudioClip>();
        private Dictionary<string, AudioClip> sfxClipDict = new Dictionary<string, AudioClip>();

        [SerializeField]
        private List<AudioObject> _audioObjects;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            foreach (var clip in bgmClips)
            {
                if (clip != null && !bgmClipDict.ContainsKey(clip.name))
                {
                    bgmClipDict.Add(clip.name, clip);
                }
            }

            foreach (var clip in sfxClips)
            {
                if (clip != null && !sfxClipDict.ContainsKey(clip.name))
                {
                    sfxClipDict.Add(clip.name, clip);
                }
            }

            if (masterMixer != null)
            {
                AudioMixerGroup[] mixerGroups = masterMixer.FindMatchingGroups(string.Empty);

                foreach (var group in mixerGroups)
                {
                    if (group.name == bgmMixerGroupName)
                    {
                        bgmSource.outputAudioMixerGroup = group;
                    }
                    else if (group.name == sfxMixerGroupName)
                    {
                        sfxSource.outputAudioMixerGroup = group;
                    }
                }
            }
        }

        #region BGM Methods

        public void PlayBGM(string clipName, bool loop = true)
        {
            if (bgmClipDict.TryGetValue(clipName, out AudioClip clip))
            {
                bgmSource.clip = clip;
                bgmSource.loop = loop;
                bgmSource.Play();
            }
            else
            {
                Debug.LogWarning($"BGM '{clipName}'을 찾을 수 없습니다.");
            }
        }

        public void StopBGM()
        {
            bgmSource.Stop();
            bgmSource.clip = null;
        }

        public void SetBGMVolume(float volume)
        {
            bgmSource.volume = volume;
        }

        #endregion

        #region SFX Methods

        public void PlaySfx(string clipName)
        {
            if (sfxClipDict.TryGetValue(clipName, out AudioClip clip))
            {
                sfxSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"SFX '{clipName}'을 찾을 수 없습니다.");
            }
        }

        public void SetSfxVolume(float volume)
        {
            sfxSource.volume = volume;
        }

        #endregion

        #region Low-Pass Filter Control

        public void ApplyLowPassFilter(bool apply, float transitionTime = 1.0f)
        {
            if (masterMixer != null)
            {
                if (apply)
                {
                    masterMixer.FindSnapshot("LowPass").TransitionTo(transitionTime);
                    foreach (AudioObject audioObject in _audioObjects)
                    {
                        audioObject.ApplySlow(true);
                    }
                }
                else
                {
                    masterMixer.FindSnapshot("Normal").TransitionTo(transitionTime);
                    foreach (AudioObject audioObject in _audioObjects)
                    {
                        audioObject.ApplySlow(false);
                    }
                }
            }
        }

        #endregion
    }
}
