#region

using SWPPT3.Main.Manager;
using SWPPT3.Main.Prop;
using UnityEngine;

#endregion

namespace SWPPT3.Main.AudioLogic
{
    public class OnOffSfx : AudioObject
    {
        public override void DeActivateSound()
        {
            deActiveSource.Play();
        }

        public override void SetVolume(float volume)
        {
            activeSource.volume = volume;
            deActiveSource.volume = volume;
        }

        public void Start()
        {
            BgmManager.Instance.AddSfxObject(this);
        }

        public void OnDestroy()
        {
            BgmManager.Instance.RemoveSfxObject(this);
        }
    }
}
