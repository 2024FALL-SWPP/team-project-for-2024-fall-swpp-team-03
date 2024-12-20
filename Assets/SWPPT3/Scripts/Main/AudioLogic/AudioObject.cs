#region

using SWPPT3.Main.Prop;
using UnityEngine;

#endregion

namespace SWPPT3.Main.AudioLogic
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class AudioObject : MonoBehaviour
    {
        [SerializeField]
        private StatefulProp _statefulProp;

        [SerializeField]
        protected AudioSource activeSource;
        [SerializeField]
        protected AudioSource deActiveSource;


        public virtual void ActivateSound()
        {
            activeSource.Play();
        }
        public abstract void DeActivateSound();

        private bool isActive;

        public void Awake()
        {
            isActive = false;
        }

        public void Update()
        {
            if (!isActive && _statefulProp.State)
            {
                ActivateSound();
                isActive = true;
            }
            else if (isActive && !_statefulProp.State)
            {
                DeActivateSound();
                isActive = false;
            }
        }

        public abstract void SetVolume(float volume);
    }
}
