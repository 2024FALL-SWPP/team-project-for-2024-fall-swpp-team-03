using SWPPT3.Main.Manager;

namespace SWPPT3.Main.AudioLogic
{
    public class OnlyOnSfx : AudioObject
    {
        public override void DeActivateSound()
        {
        }

        public override void SetVolume(float volume)
        {
            activeSource.volume = volume;
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
