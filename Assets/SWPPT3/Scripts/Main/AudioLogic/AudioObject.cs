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
        protected AudioSource audioSource;

        public abstract void PlaySound();

        private bool isActive;

        private bool isPlayer;
        public void Awake()
        {
            isActive = false;
            if (_statefulProp != null)
            {
                isPlayer = false;
            }
            else
            {
                isPlayer = true;
            }
        }

        public void Update()
        {
            if (!isPlayer)
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
            else
            {
                Debug.Log("Player");
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
