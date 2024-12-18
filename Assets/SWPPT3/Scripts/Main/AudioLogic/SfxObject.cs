using System;
using SWPPT3.Main.Manager;

namespace SWPPT3.Main.AudioLogic
{
    public class SfxObject : AudioObject
    {
        public override void PlaySound()
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        public void Awake()
        {
            BgmManager.Instance.AddSfxObject(this);
        }

        public void OnDestroy()
        {
            BgmManager.Instance.RemoveSfxObject(this);
        }
    }
}
