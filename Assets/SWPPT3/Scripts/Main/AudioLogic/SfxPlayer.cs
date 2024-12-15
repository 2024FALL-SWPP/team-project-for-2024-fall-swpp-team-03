namespace SWPPT3.Main.AudioLogic
{
    public class SfxPlayer : AudioPlayer
    {
        public void PlaySound()
        {
            AudioSource.PlayOneShot(AudioSource.clip);
        }
    }
}
