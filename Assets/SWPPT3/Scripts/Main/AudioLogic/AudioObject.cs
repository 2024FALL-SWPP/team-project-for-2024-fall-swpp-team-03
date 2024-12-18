#region

using SWPPT3.Main.Prop;
using UnityEngine;

#endregion

namespace SWPPT3.Main.AudioLogic
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class AudioObject : MonoBehaviour
    {

        private StatefulProp _statefulProp;

        public void OnEnable()
        {
            _statefulProp = GetComponent<StatefulProp>();
        }

        [SerializeField]
        protected AudioSource audioSource;


        public abstract void PlaySound();

        private bool isActive = false;

        public void Update()
        {
            if (!isActive && _statefulProp.State)
            {
                PlaySound();
                isActive = true;
            }
            else if (isActive && !_statefulProp.State)
            {
                isActive = false;
                StopSound();
            }
        }

        public void SetVolume(float volume)
        {
            audioSource.volume = volume;
        }

        public virtual void StopSound()
        {

        }
    }
}
