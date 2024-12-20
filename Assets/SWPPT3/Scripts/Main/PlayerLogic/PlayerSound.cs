using SWPPT3.Main.Manager;
using UnityEditor;
using UnityEngine;

namespace SWPPT3.Main.PlayerLogic
{
    public enum Sounds
    {
        Awake,
        Jump,
        Collision,
        Gas,
        Change,
        Item
    }
    public class PlayerSound : MonoBehaviour
    {
        [SerializeField] private AudioSource _awakeSFX;

        [SerializeField] private AudioSource _jumpSFX;
        [SerializeField] private AudioSource _collisionSFX;
        [SerializeField] private AudioSource _gasSFX;
        [SerializeField] private AudioSource _changeSFX;
        [SerializeField] private AudioSource _itemSFX;

        public void PlaySound(Sounds sound)
        {
            switch (sound)
            {
                case Sounds.Awake:
                    AwakeSound();
                    break;
                case Sounds.Jump:
                    JumpSound();
                    break;
                case Sounds.Collision:
                    CollideSound();
                    break;
                case Sounds.Gas:
                    GasSound();
                    break;
                case Sounds.Change:
                    ChangeSound();
                    break;
                case Sounds.Item:
                    ItemSound();
                    break;
            }
        }

        private void AwakeSound()
        {
            _awakeSFX.Play();
        }

        private void JumpSound()
        {
            _jumpSFX.Play();
        }

        private void ChangeSound()
        {
            _changeSFX.Play();
        }

        private void GasSound()
        {
            _gasSFX.Play();
        }

        private void CollideSound()
        {
            _collisionSFX.Play();
        }

        private void ItemSound()
        {
            _itemSFX.Play();
        }

        public void Awake()
        {
            _awakeSFX.volume = BgmManager.Instance.SFXVolume;
            _jumpSFX.volume = BgmManager.Instance.SFXVolume;
            _changeSFX.volume = BgmManager.Instance.SFXVolume;
            _gasSFX.volume = BgmManager.Instance.SFXVolume;
            _collisionSFX.volume = BgmManager.Instance.SFXVolume;
            _itemSFX.volume = BgmManager.Instance.SFXVolume;

        }
    }
}
