#region

using UnityEngine;

#endregion

namespace SWPPT3.Main.AudioLogic
{
    [CreateAssetMenu(fileName = "BGMScript", menuName = "SWPPT3/Scripts/BGMScript")]
    public class BGMScript : ScriptableObject
    {
        [SerializeField] private float _bgmVolume;
        [SerializeField] private float _sfxVolume;
        public float BgmVolume {get => _bgmVolume; set => _bgmVolume = value; }
        public float SfxVolume { get => _sfxVolume; set => _sfxVolume = value; }
    }
}

