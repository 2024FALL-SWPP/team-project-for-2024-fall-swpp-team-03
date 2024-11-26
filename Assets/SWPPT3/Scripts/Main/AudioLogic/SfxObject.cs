namespace SWPPT3.Main.AudioLogic
{
    public class SfxObject : AudioObject
    {
        public void PlaySound()
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}
